#!/usr/bin/env bash
############################################
#author			Olivier Pasche
#date			2012-06-20
#title			admin_config.sh
#description	Suppress Ds_Store, create mail attached documents icon, lock Saved Application State folder, suppress launch of iCloud at first login in admin account.
#usage			bash admin_config.sh
#note			-
############################################

#suppression fichiers DS_store:
defaults write com.apple.desktopservices DSDontWriteNetworkStores true
killall Finder
#documents attachés sous forme d'icône dans mail:
defaults write com.apple.mail DisableInlineAttachmentViewing -boolean YES
#dock en 2D:
defaults write com.apple.dock no-glass -boolean YES 
killall Dock
#suppression mémoire documents utilisés:
sudo chflags nouchg /Users/admin/Library/Saved\ Application\ State/
sudo rm -rf /Users/admin/Library/Saved\ Application\ State/*
sudo chflags uchg /Users/admin/Library/Saved\ Application\ State/
#empêcher ouverture de iCloud:
sudo defaults write /System/Library/User\ Template/Non_localized/Library/Preferences/com.apple.SetupAssistant DidSeeCloudSetup -boolean YES
exit