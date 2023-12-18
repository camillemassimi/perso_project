/* 
Défi de code - Implémenter les règles du défi FizzBuzz
Voici les règles du défi FizzBuzz que vous devez implémenter dans votre projet de code :
    Générez des valeurs de sortie comprises entre 1 et 100, une par ligne, à l’intérieur du bloc de code d’une instruction d’itération.
        - Quand la valeur actuelle est divisible par 3, afficher le terme Fizz en regard du nombre.
        - Quand la valeur actuelle est divisible par 5, afficher le terme Buzz en regard du nombre.
        - Quand la valeur actuelle est divisible à la fois par 3 et par 5, afficher le terme FizzBuzz en regard du nombre.
*/
    
for (int i = 1; i<101; i++)
{
    if((i % 3 == 0) && (i % 5 == 0))
    {
        Console.WriteLine($"{i} - FizzBuzz");
    }
    else if (i % 5 == 0)
    {
        Console.WriteLine($"{i} - Buzz");
    }
    else if (i % 3 == 0)
    {
        Console.WriteLine($"{i} - Fizz");
    }
    else
    {
        Console.WriteLine($"{i}");
    }
}
