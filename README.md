# Notatnik

>An app for note-taking. Effortlessly create *Markdown* notes *by yourself*, or *collaborate and share* with coworkers, your manager or friends.
>Created with **XAML** and **C#**, using **WPF** and **LINQ** frameworks. 
>Includes **Microsoft SQL Server Database**.


## Installation


1. Download the project, and try compiling it — it won't launch at first but *that's kind of okay*.
2. Navigate to the `Notatnik/Notatnik/bin/Debug>net8.0-windows7.0` or another folder where the executive file is located.
3. Once there, create a text file **appsettings.json**  with the following contents:<br>
`{`<br>
`"GitHub": {`<br>
`		"ClientId": "YOUR_GITHUB_CLIENT_ID",`<br>
`		"ClientSecret": "YOUR_GITHUB_CLIENT_SECRET"`<br>
`		}`<br>
`}`<br>
5. You are all set! Launch the *Notatnik.exe* file.


## How to use *Notatnik*:
 
 
### Logging in

- Log in using your GitHub account, *Notatnik* will automatically redirect you to your browser to log in. 
- You may also choose to use the application without logging in --- but beware, you won't be able to share created files trough the app or download files from other users.


### Choosing your view

- *Organisation* view --- This module will allow you to view your organisation, and manage it if you have the administrator permissions.
- *Notepad* view --- This is 'the meat' of the application, allowing you to create, edit and synchronize markdown files.


### Working with a file

Now you are free to create a directory or a file and edit it's content. Use the upper field to write in your file. The real-time preview will show you the markdown final product.


# Synchronization



Synchronization is one of the main features of *Notatnik*. It enables you to synchronize any file in your workspace with other files stored in your organization's database. This allows you to keep writing on other devices, collaborate with people you share an organization with and be up to date with the newest updates. You can synchronize your work with these mechanisms:

- ### Sending the files

- ### Downloading the files

## Sending

Using this option allows you to instantly send your changes in files and directories *(as well as any deletions and additions to the file tree)* to the database of your organization. These changes will be accesible to other users in the same org.

## Downloading

Using this option lets you synch your workspace with the  files and directories from your organisation's database.

# Markdown:


Our goal is to make markdown easily accessible, hence *Notatnik* features a simplified way to create elements in markdown for the user, while staying true to its structure and keeping the data the same as for a standard **.md** file upon saving.
So far following markdown functionalities have been implemented:

- Styling text --- **bold** and *italic*
- Headers 

> We are working on implementing many more markdown elements, more on this in the next section.


# Future development and fixes


#### We are working to make this project better, so there are many improvements on the horizon, the ones on the forefront of our minds at the moment are:

- **Fix:** `Deleting users from an org in the organisation view`
- **Expanding on user hierarchy** --- adding roles such as head admins, etc.
- **Ability to add organisations** from the application
<br><br><br><br><br><br>
##### Got questions? Suggestions? Contact the team at *notat-mail@snail.com*.
