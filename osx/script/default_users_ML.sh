#!/usr/bin/env bash
############################################
#author			Olivier Pasche
#date			2012-06-20
#title			default_users_ML.sh
#version		1.0
#description	Create the default user template. Lunched for a new installation
#usage			bash default_users_ML.sh
#note			-
############################################

BASE="/System/Library/User Template"

echo "Deleting existing template folder"
rm -Rf "$BASE/French.lproj/Library"
#Copier le dossier Library de l'utilisateur (admin dans le dossier French.lproj et supprimer le dossier English.lproj:
echo "Copying Library from admin to French template"
ditto -rsrcFork -V "/Users/admin/Library" "$BASE/French.lproj/Library"
echo "Deleting English template"
rm -Rf "$BASE/English.lproj"
############################################
# Supprimer tout ce qui est inutile dans la bibliothèque French.lproj
echo "Deleting unnecessary stuff from template folder"
rm -Rf "$BASE/French.lproj/Library/Accounts/"
rm -Rf "$BASE/French.lproj/Library/Address Book Plug-Ins/"
rm -Rf "$BASE/French.lproj/Library/Caches/"
rm -Rf "$BASE/French.lproj/Library/Calendars/"
rm -Rf "$BASE/French.lproj/Library/Colors/"
rm -Rf "$BASE/French.lproj/Library/Container/"
rm -Rf "$BASE/French.lproj/Library/Cookies/"
rm -Rf "$BASE/French.lproj/Library/Fonts Disabled/"
rm -Rf "$BASE/French.lproj/Library/Keychains/"
rm -Rf "$BASE/French.lproj/Library/LaunchAgents/"
rm -Rf "$BASE/French.lproj/Library/Logs/"
rm -Rf "$BASE/French.lproj/Library/Mails/"
rm -Rf "$BASE/French.lproj/Library/Messages/"
rm -Rf "$BASE/French.lproj/Library/PreferencePanes/"
rm -Rf "$BASE/French.lproj/Library/PubSub/"
rm -Rf "$BASE/French.lproj/Library/Spelling/"
rm -Rf "$BASE/French.lproj/Library/SyncedPreferences/"
rm -Rf "$BASE/French.lproj/Library/Thundebirds/"
rm -Rf "$BASE/French.lproj/Library/Application Support/CrashReporter/"
rm -Rf "$BASE/French.lproj/Library/Application Support/iCloud/"
############################################
# Rajout des elements a probleme dans la bibliothèque French.lproj
echo "Adding necessary stuff to template folder"
mkdir "$BASE/French.lproj/Library/Calendars/"
mkdir "$BASE/French.lproj/Library/ColorSync/"
mkdir "$BASE/French.lproj/Library/Components/"
mkdir "$BASE/French.lproj/Library/KeyBindings/"
mkdir "$BASE/French.lproj/Library/Keychains/"
mkdir "$BASE/French.lproj/Library/QuickLook/"
mkdir "$BASE/French.lproj/Library/Application Support/CrashReporter/"
mkdir "$BASE/French.lproj/Library/Application Support/iCloud/"
############################################
# suppression des préf. liées à l'uuid de la machine
rm -Rf "$BASE/French.lproj/Library/Preferences/ByHost"
# nettoyage des applications ouvertes au login
rm -f "$BASE/French.lproj/Library/Preferences/com.apple.loginwindow.*"
############################################
#Recrééer le dossier English.lproj:
echo "Copying French template folder to English template folder"
cp -Rf "$BASE/French.lproj" "$BASE/English.lproj"
#Modifier le groupe du dossier copié:
echo "Removing readonly"
chflags nouchg "$BASE/French.lproj/Library/Saved Application State"
echo "Changing owner on template folders"
chown -R root:wheel "$BASE/French.lproj" 
chown -R root:wheel "$BASE/English.lproj"
#Changer les droits sur le dossier:
echo "Changing permisson on template folders"
chmod -R 700 "$BASE/French.lproj" 
chmod -R 700 "$BASE/English.lproj"
echo "Changing Saved Application State folder to readonly"
chflags uchg "$BASE/French.lproj/Library/Saved Application State"
chflags uchg "$BASE/English.lproj/Library/Saved Application State"
echo "Terminé !"
exit