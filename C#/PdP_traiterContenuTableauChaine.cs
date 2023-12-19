/* 
Projet de code 3 – Écrire du code qui traite le contenu d’un tableau de chaînes

Voici les conditions que votre troisième projet de programmation doit implémenter :

     - Votre solution doit utiliser le tableau de chaînes suivant pour représenter l’entrée de votre logique de programmation :
          - string[] myStrings = new string[2] { "I like pizza. I like roast chicken. I like salad", "I like all three of the menu choices" };
     - Votre solution doit déclarer une variable entière nommée periodLocation qui peut être utilisée pour contenir l’emplacement du caractère point dans une chaîne.
     - Votre solution doit inclure une boucle foreach ou for externe qui peut être utilisée pour traiter chaque élément de chaîne dans le tableau. La variable de chaîne que vous allez traiter dans les boucles doit être nommée myString.
     - Dans la boucle externe, votre solution doit utiliser la méthode IndexOf() de la classe String pour obtenir l’emplacement du premier caractère point dans la variable myString. L’appel de méthode doit ressembler à ceci : myString.IndexOf("."). S’il n’existe aucun point dans la chaîne, la valeur -1 est retournée.
     - Votre solution doit inclure une boucle do-while ou while interne qui peut être utilisée pour traiter la variable myString.
     - Dans la boucle interne, votre solution doit extraire et afficher (écrire dans la console) chaque phrase contenue dans chacune des chaînes traitées.
     - Dans la boucle interne, votre solution ne doit pas afficher le caractère point.
     - Dans la boucle interne, votre solution doit utiliser les méthodes Remove(), Substring() et TrimStart() pour traiter les informations de chaîne.
*/

string[] myStrings = new string[2] { "I like pizza. I like roast chicken. I like salad", "I like all three of the menu choices" };

int periodLocation = 0; //emplacement du caractère point dans une chaîne

for (int i = 0; i < myStrings.Length; i++) //on utilise for pour pouvoir modifier la chaine
{
     string myString = myStrings[i];
     periodLocation = myString.IndexOf("."); //on recupere l'index d'où est la premiere virgule (renvoie -1 si pas de virgule)

     //cas avec virgule
     if (periodLocation != -1)
     {
          do
          {
               Console.WriteLine(myString.Substring(0, periodLocation)); //on renvoie la partie de la chaine qui est avant la virgule
               myString = myString.Substring(periodLocation+1).Trim(); //on conserve la partie de la chaine qui est après la virgule
               periodLocation = myString.IndexOf("."); //on recupère le nouvel indice de la chaine ou est la virgule

          } while (periodLocation != -1); 

          Console.WriteLine(myString);  //pour renvoyer la dernière partie après la dernière virgule
     } 
     
     //cas sans virgule
     else
     {
        Console.WriteLine(myString);  
     }
}