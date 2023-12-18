/*
Défi de code - Écrire du code pour implémenter les règles du jeu

Voici les règles du jeu de combat que vous devez implémenter dans votre projet de code :
- Vous devez utiliser l’instruction do-while ou while comme boucle de jeu externe.
- Le héros et le monstre commencent avec 10 points de vie.
- Toutes les attaques ont une valeur comprise entre 1 et 10.
- Le héros attaque en premier.
- Afficher le nombre de points que le monstre a perdus et ses points restants.
- Si les points de vie du monstre sont supérieurs à 0, celui-ci peut attaquer le héros.
- Afficher le nombre de points que le héros a perdus et ses points restants.
- Continuer cette séquence d’attaques jusqu’à ce que les points de vie du monstre ou du héros soient inférieurs ou égaux à zéro.
- Afficher le gagnant.
*/

Console.WriteLine("Défi Do et While");

int vie_heros = 10;
int vie_monstre = 10;

Random number = new Random();
int pts_vie_perdu = 0;
pts_vie_perdu = number.Next(1,11);

while ((vie_heros > 0) & (vie_monstre > 0))
{
    pts_vie_perdu = number.Next(1,11);
    vie_monstre -= pts_vie_perdu;
    Console.WriteLine($"Le monstre est attaqué, il perd {pts_vie_perdu} points de vie. Point de vie restant : {vie_monstre}");
    

    if (vie_monstre > 0)
    {
        pts_vie_perdu = number.Next(1,11);
        vie_heros -= pts_vie_perdu;
        Console.WriteLine($"Le héro est attaqué, il perd {pts_vie_perdu} points de vie. Point de vie restant : {vie_heros}");

        if (vie_heros <= 0)
        {
            Console.WriteLine("Le monstre a gagné !!!");
            break; 
        }

    }
    else
    {
        Console.WriteLine("Le héro a gagné !!!");
        break;
    }
}
