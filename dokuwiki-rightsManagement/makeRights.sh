#!/bin/bash

# Variables init
#########################################################################
publicManualRights=../../conf/acl.auth/publicManualRights.txt
privateManualRights=../../conf/acl.auth/privateRights.txt
tmpPublicPages=./tmpPublicPages.txt
publicACL=./publicACL.txt
privateACL=./privateACL.txt
tmpFinalACLfile=./tmpFinalACLfile
finalACLfile=../../conf/acl.auth.php

ACLdir=../../conf/acl.auth/backupFiles/$(date +%Y%m%d_%H%M)/
ACLbackupFile=${ACLdir}acl.auth.$(date +%Y%m%d_%H%M).php

# Create public rights
#########################################################################
# Parse the wiki to identify the public pages
find ./{fr,en}/public/ -name "*.txt" | xargs grep -h "\[\[" | grep -v "http" | sed -n 's/.*\[\[\([^*]*\)\]\]/\1/p' | cut -f 1 -d \| >> $tmpPublicPages
cat ./{fr,en}/sidebar.txt | grep -v "http" | sed -n 's/.*\[\[\([^*]*\)\]\]/\1/p' | cut -f 1 -d \| >> $tmpPublicPages

# Generate public file (dokuwiki style)
while read page  
do
    echo -e "$page\t@ALL\t1" >> $publicACL # Allow the page to be public
    echo -e "$page:*\t@ALL\t1" >> $publicACL # Allow the ressources (images) to be public
    echo -e "$page\t@fbm%2dwiki%2dsi%2dmodificateur%2dg\t2" >> $publicACL # Allow the group fbm-wiki-si-modificateur to modify the page
done < $tmpPublicPages

# Merge the manual public rights to the tmpPublicPages file
cat $publicManualRights >> $publicACL

# Sort the public ACL file
cat $publicACL | sort > $publicACL



# Create private rights
#########################################################################
# Create the private ACL file
cat $privateManualRights | sort > $privateACL



# Archiving rights file
#########################################################################
mkdir -p $ACLdir
cat $finalACLfile > $ACLbackupFile



# Generate final ACL file (public and private)
#########################################################################
# Merge the public ACL file and the private ACL file
cat $publicACL >> $tmpFinalACLfile
cat $privateACL >> $tmpFinalACLfile
cat $tmpFinalACLfile | sort -u > $finalACLfile
chmod 770 $finalACLfile
chown apache:apache $finalACLfile



# Delete temporary files
#########################################################################
rm $tmpPublicPages $publicACL $privateACL $tmpFinalACLfile