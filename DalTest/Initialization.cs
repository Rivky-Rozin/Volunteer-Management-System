namespace DalTest;

using DalApi;
using DO;
using System;
using System.Data;
using System.Numerics;
using System.Reflection;

public static class Initialization
{
    private static ICall? s_dalCall; //stage 1
    private static IAssignment? s_dalAssignment; //stage 1
    private static IVolunteer? s_dalVolunteer; //stage 1
    private static IConfig? s_dalConfig; //stage 1
    private static readonly Random s_rand = new();


    private static void createVolunteers()
    {

        string[] volunteerNames = { "David Rosen", "Yael Cohen", "Moshe Levi", "Shiran Bar", "Noa Amar", "Lior Avraham" };
        string[] volunteerPhones = { "0501234567", "0527654321", "0549876543", "0531237894", "0505678910", "0523456789" };
        string[] volunteerEmails = {"david@example.com", "yael@example.com","moshe@example.com","shiran@example.com","noa@example.com","lior@example.com"};
        string[] volunteerAddresses = {"Jerusalem","Tel Aviv","Haifa","Beer Sheva","Ashdod","Netanya"};

        for (int index = 0; index < volunteerNames.Length; index++)
            {
            int id;
            do
                id = s_rand.Next(200000000, 400000000);
            while (s_dalVolunteer!.Read(id) != null);
            string name = volunteerNames[index];
            string phone = volunteerPhones[index];
            string email = volunteerEmails[index];
            string? address =  volunteerAddresses[index] ;
            double? latitude = index % 2 == 0 ? s_rand.NextDouble() * 2 + 31 : null; // בין 31 ל-33
            double? longitude = index % 2 == 0 ? s_rand.NextDouble() * 2 + 34 : null; // בין 34 ל-36

            Enum role = index % 2 == 0 ? VolunteerRole.Manager : VolunteerRole.Regular;
            bool isActive = index % 2 == 0;

            Enum distanceKind = index % 2 == 0 ? DistanceKind.Aerial : DistanceKind.Ground;
            double? maxDistance = index % 2 == 0 ? s_rand.Next(1, 50) : null; // טווח בין 1 ל-50 ק"מ
          
            s_dalVolunteer!.Create(new(id,name,phone, email,role,isActive,distanceKind,address,latitude,longitude,null,maxDistance));
        }
    }
    private static void createCalls()
    {
        
    }

    private static void createAssignments()
    {

    }
}
