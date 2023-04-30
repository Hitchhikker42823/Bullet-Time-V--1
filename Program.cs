//Define Global Variables
int[] mapSize = { 10, 10 }; //map size (x,y)
int[] p1Position = {0, 0, 2, 3}; //Player 1's position. (x, y, rotation, Enemy Health) (0==up, 1==right, 2==down, 3==left) 
//Really doesn't make sense to have Enemy health in here, but I don't know how to return multilple variables from one method so it has to stay this way
int[] EnemyPosition = { 9, 9, 0}; //Enemy's position. (x, y, rotation) (0==up, 1==right, 2==down, 3==left)
int EnemyMoveChance = 4; //D10 must roll higher than this for enemy to move. Decrease for more difficulty

static void DrawMap(int[] mapSize, int[] p1Position, int[] EnemyPosition) //Makes a function to draw the board with players on it
{
    for (int mapDrawY = 0; mapDrawY < mapSize[1]; mapDrawY += 1) //Repeats the line creation loop until the map is tall enough
    {
        for (int mapDrawX = 0; mapDrawX < mapSize[0]; mapDrawX += 1) //Creates a line of characters to make the map
        {
            if (!((mapDrawX == p1Position[0] & mapDrawY == p1Position[1]) | (mapDrawX == EnemyPosition[0] & mapDrawY == EnemyPosition[1]))) //checks if player or the enemy is in the location we are trying to draw
            {
                Console.Write(" '"); //prints blank spaces
            }
            if(mapDrawX == p1Position[0] & mapDrawY == p1Position[1]) //prints the player. The if statements nested in this print the proper orientation of the player.
            {
                if (p1Position[2] == 0)
                {
                    Console.Write(" ^");
                }
                if (p1Position[2] == 1)
                {
                    Console.Write(" >");
                }
                if (p1Position[2] == 2)
                {
                    Console.Write(" v");
                }
                if (p1Position[2] == 3)
                {
                    Console.Write(" <");
                }
            }
            if (mapDrawX == EnemyPosition[0] & mapDrawY == EnemyPosition[1]) //prints the enemy. I don't like that it's using letters but there's not really another rotatable character
            {
                if (EnemyPosition[2] == 0)
                {
                    Console.Write(" u");
                }
                if (EnemyPosition[2] == 1)
                {
                    Console.Write(" r");
                }
                if (EnemyPosition[2] == 2)
                {
                    Console.Write(" d");
                }
                if (EnemyPosition[2] == 3)
                {
                    Console.Write(" l");
                }
            }
        }
        Console.Write("\n"); //goes to next row
    }
    Console.Write("\n"); //adds a space after the board to look nicer
}

static int[] MovePlayer(int[] mapSize, int[] p1Position, int[] EnemyPosition)//takes input and returns a set of updated player coordinates
{
    string PlayerInput = "";
    int[] newP1Position = p1Position;
    int[] BulletPositions = { 10, 10, 10, 10 };
    while (!(PlayerInput == "w" | PlayerInput == "a" | PlayerInput == "s" | PlayerInput == "d" | PlayerInput == "f")) //validates input
    {
        PlayerInput = Console.ReadLine();
    }
    if (PlayerInput == "w") //Moves foward based off of rotation; check wall and enemy position
    {
        if (p1Position[2] == 0 & p1Position[1] > 0 & !(p1Position[0] == EnemyPosition[0] & p1Position![1] == EnemyPosition[1] - 1))
        {
            newP1Position[1]--;
        }
        if (p1Position[2] == 1 & p1Position[0] < mapSize[0] - 1 & !(p1Position[1] == EnemyPosition[1] & p1Position![0] == EnemyPosition[0] + 1))
        {
            newP1Position[0]++;
        }
        if (p1Position[2] == 2 & p1Position[1] < mapSize[1] - 1 & !(p1Position[0] == EnemyPosition[0] & p1Position![1] == EnemyPosition[1] + 1))
        {
            newP1Position[1]++;
        }
        if (p1Position[2] == 3 & p1Position[0] > 0 & !(p1Position[1] == EnemyPosition[1] & p1Position![0] == EnemyPosition[0] - 1))
        {
            newP1Position[0]--;
        }
    }
    if (PlayerInput == "s") //Moves backward based off of rotation; check wall and enemy position
    {
        if (p1Position[2] == 0 &  p1Position[1] < mapSize[0] - 1 & !(p1Position[0] == EnemyPosition[0] & p1Position![1] == EnemyPosition[1] + 1))
        {
            newP1Position[1]++;
        }
        if (p1Position[2] == 1 & p1Position[0] > 0 & !(p1Position[1] == EnemyPosition[1] & p1Position![0] == EnemyPosition[0] - 1))
        {
            newP1Position[0]--;
        }
        if (p1Position[2] == 2 & p1Position[1] > 0 & !(p1Position[0] == EnemyPosition[0] & p1Position![1] == EnemyPosition[1] + 1))
        {
            newP1Position[1]--;
        }
        if (p1Position[2] == 3 &  p1Position[0] < mapSize[1] - 1 & !(p1Position[1] == EnemyPosition[1] & p1Position![0] == EnemyPosition[0] + 1))
        {
            newP1Position[0]++;
        }
    }
    if (PlayerInput == "a") //Turns left
    {
        newP1Position[2]--;
    }
    if (PlayerInput == "d") //Turns right
    {
        newP1Position[2]++;
    }
    if (PlayerInput == "f")//shoots the 2 squares in front of it
    {
        BulletPositions = CreateBullets(p1Position); //Originally intended for graphics, but would need to return 2 things, which I don't know how to do. Just makes hitboxes.
        newP1Position[3] = DamageEnemy(p1Position, EnemyPosition, BulletPositions);
    }

    if (newP1Position[2] == 4) //loops right turn value back around so that it works properly
    {
        newP1Position[2] = 0;
    }
    if (newP1Position[2] == -1) //loops left turn value back around so that it works properly
    {
        newP1Position[2] = 3;
    }
    return newP1Position; //returns the edited position - will be set as the current position somewhere else
}

static int[] MoveEnemy(int[] mapSize, int[] p1Position, int[] EnemyPosition, int EnemyMoveChance) //Decides where the enemy should move and then does it
{
    int[] newEnemyPosition = EnemyPosition;

    int randomNumber; //These three lines roll a D10
    Random RNG = new Random(); //The AI will only move if it's >EnemyMoveChance
    randomNumber = RNG.Next(1, 10); //This makes the AI easier since it was perfectly chasing the player. Can be tuned for difficulty
    if (Math.Abs(p1Position[0] - EnemyPosition[0]) > Math.Abs(p1Position[1] - EnemyPosition[1]) & randomNumber > EnemyMoveChance) //are they further horizontally than vertically? Only do this some the time
    {
        if (p1Position[0] < EnemyPosition[0] & EnemyPosition[2] < 2) //turn left if they're on our left and we are up or right
        {
            newEnemyPosition[2]--;
        }
        if (p1Position[0] < EnemyPosition[0] & EnemyPosition[2] == 2) //turn right if they're on our left and we are down
        {
            newEnemyPosition[2]++;
        }
        if (p1Position[0] < EnemyPosition[0] & EnemyPosition[2] == 3 & EnemyPosition[0] > 0) //go foward if they're on our left and we are facing left; check for wall
        {
            newEnemyPosition[0]--;
        }
        if (p1Position[0] > EnemyPosition[0] & EnemyPosition[2] < 1) //turn right if they're on our right and we are up
        {
            newEnemyPosition[2]++;
        }
        if (p1Position[0] > EnemyPosition[0] & EnemyPosition[2] == 2) //turn left if they're on our right and we are down or left
        {
            newEnemyPosition[2]--;
        }
        if (p1Position[0] > EnemyPosition[0] & EnemyPosition[2] == 1 & EnemyPosition[0] < mapSize[0] - 1) //go foward if they're on our right and we are facing right; check for wall
        {
            newEnemyPosition[0]++;
        }
    }
    if (Math.Abs(p1Position[0] - EnemyPosition[0]) <= Math.Abs(p1Position[1] - EnemyPosition[1]) & randomNumber > EnemyMoveChance) //Check if they're further horizontally. Only do this some of the time
    {
        if (p1Position[1] < EnemyPosition[1] & (EnemyPosition[2] ==2 | EnemyPosition[2] == 1)) //turn left if they're above us and we're right or down; coded like this to exclude up
        {
            newEnemyPosition[2]--;
        }
        if (p1Position[1] < EnemyPosition[1] & EnemyPosition[2] == 3) //turn right if they're on above us and we're left
        {
            newEnemyPosition[2]++;
        }
        if (p1Position[1] < EnemyPosition[1] & EnemyPosition[2] == 0 & EnemyPosition[1] > 0) //go foward if they're above us and we are facing up; check for wall
        {
            newEnemyPosition[1]--;
        }
        if (p1Position[1] > EnemyPosition[1] & (EnemyPosition[2] ==0 | EnemyPosition[2] == 1)) //turn right if they're below us and we're right or up; coded like this to exclude down
        {
            newEnemyPosition[2]++;
        }
        if (p1Position[1] > EnemyPosition[1] & EnemyPosition[2] == 3) //turn left if they're on below us and we're left
        {
            newEnemyPosition[2]--;
        }
        if (p1Position[1] > EnemyPosition[1] & EnemyPosition[2] == 2 & EnemyPosition[1] < mapSize[1]-1) //go foward if they're below us and we are facing down; check for wall
        {
            newEnemyPosition[1]++;
        }
    }
    if (newEnemyPosition[2] == 4) //loops right turn value back around so that it works properly
    {
        newEnemyPosition[2] = 0;
    }
    if (newEnemyPosition[2] == -1) //loops left turn value back around so that it works properly
    {
        newEnemyPosition[2] = 3;
    }
    return newEnemyPosition; //returns the edited position - will be set as the current position somewhere else
}

static int DamageEnemy(int[] p1Position, int[] EnemyPosition,  int[] BulletPositions) //creates visuals for gun firing and then decrements EnemyHealth if necessary
{
    int newEnemyHealth = p1Position[3]; //placeholder for enemy health
    if ((BulletPositions[0] == EnemyPosition[0] & BulletPositions[1] == EnemyPosition[1]) | (BulletPositions[2] == EnemyPosition[0] & BulletPositions[3] == EnemyPosition[1]))
    {
        newEnemyHealth--; //if statement above checks if enemy is intersecting with either bullet, then decrements health accordingly
    }
    return newEnemyHealth;
}

static int[] CreateBullets(int[] p1Position)
{
    int[] newBulletPositions = { 10, 10, 10, 10 };
    if (p1Position[2] == 0) //these four if statements create bullets ont the board based off of player rotation. Could be used for graphics but I'm currently unwilling to
    {
        newBulletPositions[0] = p1Position[0]; //Yes. you do in fact have to set every index independently (as far as I can tell)
        newBulletPositions[1] = p1Position[1] - 1; // makes (x1, y1, x2, y2)
        newBulletPositions[2] = p1Position[0];
        newBulletPositions[3] = p1Position[1] - 2;
    }
    if (p1Position[2] == 1)
    {
        newBulletPositions[0] = p1Position[0] + 1;
        newBulletPositions[1] = p1Position[1];
        newBulletPositions[2] = p1Position[0] + 2;
        newBulletPositions[3] = p1Position[1];
    }
    if (p1Position[2] == 2)
    {
        newBulletPositions[0] = p1Position[0];
        newBulletPositions[1] = p1Position[1] + 1;
        newBulletPositions[2] = p1Position[0];
        newBulletPositions[3] = p1Position[1] + 2;
    }
    if (p1Position[2] == 3)
    {
        newBulletPositions[0] = p1Position[0];
        newBulletPositions[1] = p1Position[1] - 1;
        newBulletPositions[2] = p1Position[0];
        newBulletPositions[3] = p1Position[1] - 2;
    }
    return newBulletPositions;
}

//intro screen
Console.WriteLine("Welcome to Bullet Time! \nProduced by Gator Beach Studios");
Console.WriteLine("Build 0 - 4/29/2023 \nPress Ctrl+C to quit\n\n");
DrawMap(mapSize, p1Position, EnemyPosition);
Console.WriteLine("You Control the arrow!\nThe enemy is the letter\n(u=facing up, r=facing right, d=facing down, l=facing left)\n\n");
Console.WriteLine("w=Foward\na=Turn Left\ns=Back\nd=Turn Right\nMake your first move:");


while ((p1Position[3] > 0) & !(p1Position[0] == EnemyPosition[0] & p1Position[1] == EnemyPosition[1])) //main gameloop
{
    p1Position = MovePlayer(mapSize, p1Position, EnemyPosition); //Player Moves (Needs Enemy Health to call Gun Function)
    EnemyPosition = MoveEnemy(mapSize, p1Position, EnemyPosition, EnemyMoveChance); //Enemy moves
    
    Console.Clear(); //Clears console in preparation for board
    Console.WriteLine("\n\n"); //Formats console in preparation for board
    DrawMap(mapSize, p1Position, EnemyPosition); //Draws next Frame
   
    Console.WriteLine("The Enemy's Health is:  " + p1Position[3] + "\n"); //Prints enemy's health - would be player HUD but this is all we need to show
}
if (p1Position[0] == EnemyPosition[0] & p1Position[1] == EnemyPosition[1]) //lose condition
{
    Console.WriteLine("You were hit!!\nGame Over!!\n");
    System.Threading.Thread.Sleep(2000); //waits before ending program so that you don't immediately close it when inputting fast
}
if (p1Position[3] == 0) //win condition
{
    Console.WriteLine("You Win!!\n");
    System.Threading.Thread.Sleep(2000); //waits before ending program so that you don't immediately close it when inputting fast
}