namespace DalTest;

using Dal;
using DalApi;
using DO;
using System;

public static class Initialization
{
    private static ICall? s_dalCall = new CallImplementation(); //stage 1
    private static IAssignment? s_dalAssignment= new AssignmentImplementation(); //stage 1
    private static IVolunteer? s_dalVolunteer= new VolunteerImplementation(); //stage 1
    private static IConfig? s_dalConfig= new ConfigImplementation(); //stage 1
    private static readonly Random s_rand = new();



    public static void createVolunteers()
    {

        string[] volunteerNames = { "David Rosen", "Yael Cohen", "Moshe Levi", "Shiran Bar", "Noa Amar", "Lior Avraham" };
        string[] volunteerPhones = { "0501234567", "0527654321", "0549876543", "0531237894", "0505678910", "0523456789" };
        string[] volunteerEmails = { "david@example.com", "yael@example.com", "moshe@example.com", "shiran@example.com", "noa@example.com", "lior@example.com" };
        string[] volunteerAddresses = { "Jerusalem", "Tel Aviv", "Haifa", "Beer Sheva", "Ashdod", "Netanya" };

        for (int index = 0; index < volunteerNames.Length; index++)
        {
            int id;
            do
                id = s_rand.Next(200000000, 400000000);
            while (s_dalVolunteer!.Read(id) != null);
            string name = volunteerNames[index];
            string phone = volunteerPhones[index];
            string email = volunteerEmails[index];
            string? address = volunteerAddresses[index];
            double? latitude = index % 2 == 0 ? s_rand.NextDouble() * 2 + 31 : null; // בין 31 ל-33
            double? longitude = index % 2 == 0 ? s_rand.NextDouble() * 2 + 34 : null; // בין 34 ל-36

            Enum role = index % 5 == 0 ? VolunteerRole.Manager : VolunteerRole.Regular;
            bool isActive = index % 2 == 0;

            Enum distanceKind = index % 2 == 0 ? DistanceKind.Aerial : DistanceKind.Ground;
            double? maxDistance = index % 2 == 0 ? s_rand.Next(1, 50) : null; // טווח בין 1 ל-50 ק"מ
            Console.WriteLine(  name);
            s_dalVolunteer!.Create(new(id, name, phone, email, role, isActive, distanceKind, address, latitude, longitude, null, maxDistance));
        }
    }
    private static void createCalls()
    {

    }

    private static void createAssignments()
    {

    }

   public static void Do(ICall? dalCall, IVolunteer? dalVolunteer, IAssignment? dalAssignment, IConfig? dalConfig)
    {
        s_dalCall = dalCall ?? throw new NullReferenceException("DAL object can not be null!"); //stage 1
        s_dalVolunteer = dalVolunteer ?? throw new NullReferenceException("DAL object can not be null!"); //stage 1                                                                                              
        s_dalAssignment = dalAssignment ?? throw new NullReferenceException("DAL object can not be null!"); //stage 1                                                                                              
        s_dalConfig = dalConfig ?? throw new NullReferenceException("DAL object can not be null!"); //stage 1                                                                                              

        Console.WriteLine("Reset Configuration values and List values...");
        s_dalConfig.Reset(); //stage 1
        s_dalCall.DeleteAll(); //stage 1
        s_dalVolunteer.DeleteAll(); //stage 1
        s_dalVolunteer.DeleteAll(); //stage 1
       
                               
        Console.WriteLine("Initializing Students list ...");

        createVolunteers();
        createCalls();
        createAssignments();


    }
}
