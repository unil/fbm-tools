#!/bin/bash

#########
# Author: Mathieu Noverraz
# Date: 30.03.15
# Goal: Formater le resultat du sondage Prism pour disposer d'une ligne par licence demandée
#########

OLDIFS=$IFS
IFS=","
i=0 # compteur de ligne du fichier
csv=""

# Liste des départements (en relation avec le nom des colonnes par département)
# En cas d'ajout d'un département dans le sondage, il faut l'ajouter dans le tableau.
tabDpt[0]="Dpt"
tabDpt[1]="Dgm"
tabDpt[2]="Dof"
tabDpt[3]="Dnf"
tabDpt[4]="Dp"


# sufix et préfix des nom de variable selon le nom de la colonne du fichier csv
departPrefix="groupe"
departSufix="Autre"




while read id complet lastDateAction lunchDate departement groupeDpt groupeDptAutre groupeDgm groupeDgmAutre groupeDof groupeDofAutre groupeDnf groupeDnfAutre groupeDp groupeDpAutre username noInv1 noInv2 noInv3 noInv4 noInv5 noInv6 noInv7 noInv8 noInv9 noInv10 accord
 do
 	if [ $i -eq 0 ]; then
 		# ligne d'entête CSV
 		csv+="$id;$complet;$departement;Groupe de recherche;$username;Numero inventaire;$accord\n"
 	else
 		for d in ${tabDpt[*]}; do
 			dpt=$departPrefix$d
 			dptAutre=$dpt$departSufix

 			# Sélectionne le département
 			if [ ${!dpt} != '""' ]; then

 				# Liste tous les champs d'inventaire et créer une ligne dans le CSV par numéro d'inventaire
 				for noInv in noInv1 noInv2 noInv3 noInv4 noInv5 noInv6 noInv7 noInv8 noInv9 noInv10; do
					if [ "${!noInv}" != '""' ]; then
						csv+="$id;$complet;$departement;"

						# identifie si groupe de recherche est une autres proposition que proposées par défaut
						if [ ${!dpt} = '"Autre"' ]; then
							csv+="${!dptAutre};"
						else
							csv+="${!dpt};"
						fi

						csv+="$username;${!noInv};$accord;\n"
					fi
				done

 			fi
 		done
 	fi

 	i=$((i+1))
 done < $1
 IFS=$OLDIFS


# Write the csv with the correct encoding for Excel (western (Windows Latin 1))
year=$(date "+%Y")
nextyear=$(($year + 1))
fileName=ListeInstallationPrism_$year-$nextyear.csv
echo -e $csv | sed s/\"//g | iconv -f UTF-8 -t ISO-8859-1 > $fileName
echo "Le fichier est disponible sous $(pwd)/$fileName"
echo "/!\ Attention!!! Le fichier peut contenir des doublons si deux utilisateurs ont entré le même numéro d'inventaire"