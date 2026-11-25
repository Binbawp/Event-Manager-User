# Event Manager – User App (.NET MAUI)

This is the **User app** for my COMP1786 coursework. It is a cross-platform app made with **.NET MAUI (C#)**. People use it to look at the events that the admin uploaded, search them, and save their favourite events.

- Student name: [YOUR NAME]
- Student ID: [YOUR ID]

It works together with the **Admin app** (Android, Java) in the `Event-Manager-Admin` folder. The Admin app uploads the events and this app reads them. **If the Admin app has not uploaded anything yet, this app will show no events.**

---

## What the app does

- Downloads all events and their sessions from the cloud (Firebase)
- **Search** by event name or by date
- Filter buttons: **All**, **Upcoming** and **Past**
- Tap an event to see its details and its list of sessions
- **Favourites**: tap the heart to save an event. Saved events show in the **Favourites** tab and stay saved on the phone even after closing the app
- **Share event** button to send the event details to another app
- Extra features: pull down to **refresh**, and an **offline mode**. The app keeps a copy of the last download, so if there is no internet it still shows the saved events with a message saying you are offline

The app has 2 tabs: **Events** and **Favourites**.

---

## What you need

| Thing | Version |
|---|---|
| .NET SDK | **.NET 10** |
| IDE | Visual Studio with the **".NET Multi-platform App UI development"** workload (I used: [YOUR VISUAL STUDIO VERSION]) |
| Android emulator or phone | Android 5.0 (API 21) or newer |
| Internet | Needed to download packages the first time and to load the events |

---

## How to compile and run it (Visual Studio)

1. **Unzip** the submission. You should have `Event-Manager-Admin` and `Event-Manager-User`.
2. Open **Visual Studio**, choose **File > Open > Project/Solution**, and open this file:

   `Event-Manager-User\EventUserApp\EventUserApp.slnx`

   If your Visual Studio cannot open the `.slnx` file, open `Event-Manager-User\EventUserApp\EventUserApp\EventUserApp.csproj` instead. It is the same project.
3. If Visual Studio says a workload is missing, open the **Visual Studio Installer**, click Modify, and tick **.NET Multi-platform App UI development**. Then restart Visual Studio.
4. Wait for the **NuGet restore** to finish (first time needs internet).
5. In the toolbar, choose **Android Emulator** as the run target. If you do not have one yet, use **Tools > Android > Android Device Manager** to create a phone (any recent Android version).
6. Press the green **Run** button. The first build takes a while.
7. The app opens on the **Events** tab.

### Other way: command line

If you prefer the terminal, open it inside the folder `Event-Manager-User\EventUserApp\EventUserApp`, and run:

```
dotnet workload install maui
dotnet build -t:Run -f net10.0-android
```

You need an Android emulator already running (or a phone connected).

### Running on Windows

The project also has a Windows target (`net10.0-windows`). To use it, Windows **Developer Mode** must be turned on. The easiest way is the Android emulator, so I suggest using that.

---

## How to use it (quick tour)

1. Make sure the **Admin app** has uploaded some events (Admin app, Settings tab, upload).
2. Open this app. The events load by themselves. Pull down to refresh at any time.
3. Type in the search box to search by name or date, or use the **All / Upcoming / Past** buttons.
4. Tap an event to open its details and sessions.
5. Tap the **heart** to add it to your favourites, and open the **Favourites** tab to see them.
6. Tap **Share event** to share the details.
7. To test the offline mode: load the events once, then turn on **airplane mode** and open the app again. You should see the saved events with an offline message.

---

## Where the data comes from

- The app reads `events.json` from a **Firebase Realtime Database**. The address is written in `Services/EventService.cs`. The Admin app uploads to the same address, which is how the two apps are connected.
- The data from the last successful download is saved in a file on the phone (this is the offline copy).
- Favourites are saved on the phone using MAUI **Preferences**, so they are stored on that device only.
- The Firebase access rules are set to allow access until **30 June 2027**. After that date Firebase would refuse the requests and the app would only show its offline copy.

---

## Project structure

```
Event-Manager-User/
└── EventUserApp/
    ├── EventUserApp.slnx                  solution file
    └── EventUserApp/                      the MAUI project
        ├── EventUserApp.csproj            project settings (.NET 10)
        ├── MauiProgram.cs                 app start-up and setup
        ├── App.xaml, AppShell.xaml        app and tab navigation
        ├── Pages/
        │   ├── EventsPage.xaml(.cs)       event list, search and filters
        │   ├── EventDetailsPage.xaml(.cs) one event, its sessions, favourite and share
        │   └── FavouritesPage.xaml(.cs)   list of favourite events
        ├── Views/EventCard.xaml(.cs)      the card used for each event in the lists
        ├── Services/
        │   ├── EventService.cs            downloads events, keeps the offline copy
        │   └── FavouritesStore.cs         saves and loads favourites
        ├── Models/                        EventDto, EventItem, SessionItem
        ├── Resources/                     icons, fonts, images and styles
        └── Platforms/                     Android, iOS, Windows, Mac files (made by the template)
```

---

## If something goes wrong

- **"No events yet" or empty list**: the Admin app may not have uploaded anything. Upload from the Admin app first, then pull down to refresh here.
- **"Could not update" message**: the app could not reach the internet, so it is showing its saved copy. Check the emulator's internet and refresh.
- **Build errors about missing workload or SDK**: install the MAUI workload and the .NET 10 SDK, as described above.
- **Strange build errors after changing the code**: delete the `bin` and `obj` folders inside `EventUserApp\EventUserApp` and build again.
- **Emulator is slow or does not start**: make sure virtualisation (Hyper-V or the Android emulator hypervisor) is turned on in Windows.

---

## Things I know could be better

I wrote about these in my report too.

- Favourites are saved **per device**, not per user account.
- There are no automated tests.
- The screens are made for phones. Tablets, iOS and Mac were not tested [EDIT THIS LINE IF YOU TESTED THEM].
- The app has no login, and it only reads data, so it depends on how the Firebase rules are set.