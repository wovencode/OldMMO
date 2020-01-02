==========================================================================================
OPEN MMO - QUICKSTART
==========================================================================================
1. Launch Unity 3d
2. Create a fresh, blank Unity 3d project (set to 3d)
3. In the unity editor menu, GoTo: File -> Project Settings -> Player
4. Therein locate Configuration -> Scripting Runtime Version and change C# version to 4.6
5. Restart Unity and re-open your project
6. Import OpenMMO into your project
7. Switch to the included starting scene in OpenMMO -> Data -> Scenes -> Init
8. Hit "Play" to launch the project in Host+Play mode using local SQLite.

------------------------------------------------------------------------------------------
HOW TO SWITCH BETWEEN CLIENT AND SERVER BUILDS
------------------------------------------------------------------------------------------
1. In your project hierarchy GoTo: Data -> Resources -> Configuration
2. Check "isServer" when running/building a server and uncheck "isClient".
3. Check "isClient" when running/building a client and uncheck "isServer".
4. Check both "isServer" and "isClient" when running/building a hybrid for Host + Play.
5. Run or build your project, OpenMMO takes care of the rest.

------------------------------------------------------------------------------------------
HOW TO CHANGE THE LOGIN SERVER ADDRESS
------------------------------------------------------------------------------------------
1. In your scene hierarchy GoTo: "Network Manager"
2. Select it and in the inspector edit the IP address and the port.
3. Your client builds will use the IP and port you enter there as login server.

------------------------------------------------------------------------------------------
HOW TO CHANGE THE DATABASE SYSTEM
------------------------------------------------------------------------------------------
1. In your scene hierarchy GoTo: "Network Manager" -> Unfold it -> Locate "Database Manager"
2. Choose the database system of your liking (SQLite or mySQL).
3. SQLite does not require anymore setup, you can build and run right away.
4. mySQL requires setup of a mySQL server, enter your credentials in the "Database Manager"
5. You can even switch database systems during development, for example to develop
locally using SQLite and then switch to mySQL for the live environment. Please keep in
mind that data does not get transferred from one database system to another.

------------------------------------------------------------------------------------------
HOW TO CHANGE THE EMAIL SETTINGS
------------------------------------------------------------------------------------------
1. In your scene hierarchy GoTo: "Network Manager" -> Unfold it -> Locate "Account Manager"
2. Check or uncheck some or all of the "Confirm" checkboxes to your liking.
3. If you checked at least one of the "Confirm" checkboxes, you have to setup email.
4. Otherwise you can ignore email setup as your server will never send out mail.
5. To setup eMail, you require a mail address and credentials.
6. In your scene hierarchy GoTo: "Network Manager" -> Unfold it -> Locate "Mail Manager"
7. Enter your eMail credentials.