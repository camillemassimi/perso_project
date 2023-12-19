/* 
Projet de programmation 2 : écrire du code qui valide l’entrée d’une chaîne

Voici les conditions que votre deuxième projet de programmation doit implémenter :
     - Votre solution doit inclure une itération do-while ou while.
     - Avant le bloc d’itération : votre solution doit utiliser une instruction Console.WriteLine() pour demander à l’utilisateur d’entrer l’un des trois noms de rôle suivants : Administrator, Manager ou User.
     - À l’intérieur du bloc d’itération :
          - Votre solution doit utiliser une instruction Console.ReadLine() pour obtenir l’entrée de l’utilisateur.
          - Votre solution doit vérifier que la valeur entrée correspond à l’une des trois options de rôle.
          - Votre solution doit utiliser la méthode Trim() sur la valeur d’entrée pour ignorer les espaces de début et de fin.
          - Votre solution doit utiliser la méthode ToLower() sur la valeur d’entrée pour ignorer la casse.
          - Si la valeur entrée ne correspond pas à l’une des options de rôle, votre code doit utiliser une instruction Console.WriteLine() pour inviter l’utilisateur à fournir une entrée valide.
     - Sous (après) le bloc de code d’itération : votre solution doit utiliser une instruction Console.WriteLine() pour informer l’utilisateur que sa valeur d’entrée a été acceptée.
*/

Console.WriteLine("Quel est votre rôle ? (Admin/Manager/User)");

string? choixRole;
bool etatRole = false;

do 
{
     choixRole = Console.ReadLine()?.Trim().ToLower();

     if ((choixRole == "admin") | (choixRole == "manager") | (choixRole == "user"))
     {
          etatRole = true;
          continue;
     }
     else if (string.IsNullOrEmpty(choixRole))
     {
         Console.WriteLine($"Attention, vous n'avez rien écrit. Merci de choisir l'un des rôles suivants Admin/Manager/User."); 
     }
     else
     {
          Console.WriteLine($"Attention, la valeur '{choixRole}' ne correspond pas aux choix attendus. Merci de choisir parmi Admin/Manager/User.");
     }

} while (etatRole == false);


Console.WriteLine($"Votre rôle '{choixRole}' a été accepté.");
