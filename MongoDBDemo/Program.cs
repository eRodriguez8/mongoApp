using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoDBDemo
{
	class Program
	{
		static void Main(string[] args)
		{
			MongoCRUD db = new MongoCRUD("AddressBook");

			/*
			PersonModel person = new PersonModel
			{
				FirstName = "Joe",
				LastName = "Smith",
				PrimaryAddress = new AddresModel
				{
					StreetAddres = "101 Oak Street",
					City = "Scranton",
					ZipCode = "18512"
				}
			};
			
			db.InsertRecord("Users", person);
			*/

					
			//var recs = db.LoadRecords<PersonModel>("Users");
			/*
			foreach (var rec in recs)
			{
				Console.WriteLine($"{rec.Id}: {rec.FirstName} {rec.LastName}");

				if (rec.PrimaryAddress != null)
				{
					Console.WriteLine($"{rec.PrimaryAddress.City}");
				}
			}
			*/

			//var oneRec = db.LoadRecordById<PersonModel>("User", new Guid("0a6acfca-b10c-4438-8e73-b09b015d650a"));
			//oneRec.DateOfBirth = new DateTime(1994, 05, 14, 0, 0, 0, DateTimeKind.Utc);
			//db.UpsertRecord("Users", oneRec.Id, oneRec);

			var recs = db.LoadRecords<NameModel>("Users");
			
			foreach (var rec in recs)
			{
				Console.WriteLine($"{rec.FirstName} {rec.LastName}");

			}
			
			Console.ReadLine();
		}
	}

	public class NameModel
	{
		[BsonId]
		public Guid Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
	}
	public class PersonModel
	{
		[BsonId]
		public Guid Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public AddresModel PrimaryAddress { get; set; }
		[BsonElement("dob")]
		public DateTime DateOfBirth { get; set; }
	}

	public class AddresModel
	{
		public string StreetAddres { get; set; }
		public string City { get; set; }
		public string State { get; set; }
		public string ZipCode { get; set; }
	}

	public class MongoCRUD
	{
		private IMongoDatabase db;

		public MongoCRUD(string database)
		{
			var client = new MongoClient();
			db = client.GetDatabase(database);
		}

		public void InsertRecord<T>(string table, T record)
		{
			var collection = db.GetCollection<T>(table);
			collection.InsertOne(record);
		}

		public List<T> LoadRecords<T>(string table)
		{
			var collection = db.GetCollection<T>(table);
			return collection.Find(new BsonDocument()).ToList();
		}

		public T LoadRecordById<T>(string table, Guid id) 
		{
			var collection = db.GetCollection<T>(table);
			var filter = Builders<T>.Filter.Eq("Id", id);

			return collection.Find(filter).First();
		}

		public void UpsertRecord<T>(string table, Guid id, T record)
		{
			var collection = db.GetCollection<T>(table);
			var result = collection.ReplaceOne(
				new BsonDocument("_id", id),
				record,
				new UpdateOptions { IsUpsert = true });
		}

		public void DeleteRecord<T>(string table, Guid id)
		{
			var collection = db.GetCollection<T>(table);
			var filter = Builders<T>.Filter.Eq("Id", id);
			collection.DeleteOne(filter);
		}
	}
}
