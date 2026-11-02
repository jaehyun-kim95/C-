

Cat cat = new Cat();
Animal cat2 = new Cat();

cat.Sound();
cat.move();
cat2.move();

interface IMove
{
    void move();
}

abstract class Animal : IMove
{
    virtual public void Sound() 
    {
        Console.WriteLine("울음소리");
    }

    virtual public void move() 
    {
        Console.WriteLine("움직임");
    }
}


class Cat : Animal 
{ 
    public override void Sound()
    {
        Console.WriteLine("야옹");
    }

    public override void move() 
    {
        Console.WriteLine("사뿐사뿐");
    }
}
//class Dog : Animal { }

