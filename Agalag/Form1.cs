/* Made by Gareth marks
Finished January 22
As an entertaining one or two player game
CONTROLS:
P1 uses WASD to move and Space to shoot
P2 uses IJKL to move and M to shoot
Press p to pause game
*/


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Media;
using System.IO;

namespace Agalag
{
    public partial class Form1 : Form
    {
        SolidBrush drawBrush = new SolidBrush(Color.White);
        Font gameFont = new Font("Courier New", 12);
        Font powerupFont = new Font("Impact", 16);
        Font titleFont = new Font("Liberation Mono", 30, FontStyle.Bold);

        Random rand = new Random();
        Stopwatch invincibilityWatch = new Stopwatch();//will track how long the player has been invincible
        Stopwatch speedWatch = new Stopwatch();//will track how long the player has been at double speed
        Stopwatch musicWatch = new Stopwatch();//will track how long the music has been playing

        int playerX;//player x value
        int playerY;//player y value        
        int playerHealth = 5;
        int playerLives = 3;
        int score = 0;
        int playerSpeed = 6;
        int bulletModulator = 10;//when equal to 10,  bullet will be ready to fire.
        int timeSincePowerup = 0;

        //p2 specific vars
        int player2X;//player 2 x value
        int player2Y = 100;//player 2 y value        
        int player2Health = 5;
        int player2Lives = 3;
        int p2BulletModulator = 10;

        bool gamePaused = false;
        bool playerFiring = false; //used to determine whether to play firing animation
        bool playerDying = false; //tracks whether  player is exploding
        bool gameOver = false; //tracks if game is ended
        bool playerOk = true;//tracks whether player is alive
        bool playerInvincible = false;
        bool playerFast = false;

        //p2 specific vars
        bool player2Ok = true;//tracks whether player is alive
        bool player2Firing = false;
        bool player2Dying = false;

        string fireMode = "single"; //will track the players mode of shooting
        string gameState = "title";//tracks what phase the game is in
        string winner = "";//tracks the winner in two player
        string screenText = "";//tracks what to draw to the screen on startup

        string p2FireMode = "single"; //will track the player 2 mode of shooting

        double enemySpawnRate = 1; //controls the number of enemies to spawn at each interval    

        long tracker = 0;//tracks the  number of timer repetitions passed.

        System.Windows.Media.MediaPlayer shotPlayer = new System.Windows.Media.MediaPlayer(); //defines a shot mediaplayer
        System.Windows.Media.MediaPlayer p2ShotPlayer = new System.Windows.Media.MediaPlayer(); //defines a shot mediaplayer
        System.Windows.Media.MediaPlayer boomPlayer = new System.Windows.Media.MediaPlayer(); //defines an explosion mediaplayer
        System.Windows.Media.MediaPlayer collectPlayer = new System.Windows.Media.MediaPlayer(); //defines a collect mediaplayer
        System.Windows.Media.MediaPlayer hitPlayer = new System.Windows.Media.MediaPlayer(); //defines a hit mediaplayer
        System.Windows.Media.MediaPlayer bgMusicPlayer = new System.Windows.Media.MediaPlayer(); //defines a hit mediaplayer

        int[] starXValues = new int[200];
        double[] starYValues = new double[200];
        int[] starSizeValues = new int[200];

        //arrays for highscore names and numbers
        List<int> highScores = new List<int>(new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });//list for high scores
        List<string> highScoreNames = new List<string>(new string[] { "AAA", "AAA", "AAA", "AAA", "AAA", "AAA", "AAA", "AAA", "AAA", "AAA", });//list for hs names

        List<int> bulletXValues = new List<int>(new int[] { });//list for bullet Xes
        List<int> bulletYValues = new List<int>(new int[] { });//list for bullet Ys
        List<string> bulletTypeValues = new List<string>(new string[] { });//list for bullet types

        List<int> p2BulletXValues = new List<int>(new int[] { });//list for bullet Xes
        List<int> p2BulletYValues = new List<int>(new int[] { });//list for bullet Ys
        List<string> p2BulletTypeValues = new List<string>(new string[] { });//list for bullet types

        List<int> enemyXValues = new List<int>(new int[] { });//list for enemy Xes
        List<int> enemyYValues = new List<int>(new int[] { });//list for enemy Ys
        List<string> enemyTypeValues = new List<string>(new string[] { });//list for enemy types
        List<int> enemyHealths = new List<int>(new int[] { });//list for enemy health bars
        List<int> enemyStartXes = new List<int>(new int[] { });//list for enemy start xes. useful for sine patterns in dynamic enemy types.                  

        List<int> enemyBulletXValues = new List<int>(new int[] { });//list for enemy bullet Xes
        List<int> enemyBulletYValues = new List<int>(new int[] { });//list for bullet Ys

        List<int> enemyDynamicBulletXValues = new List<int>(new int[] { });//list for dynamic type enemy bullet Xes
        List<int> enemyDynamicBulletYValues = new List<int>(new int[] { });//list for dynamic type bullet Ys
        List<int> enemyDynamicBulletXIncreases = new List<int>(new int[] { });//list for dynamic type enemy bullet Xes
        List<int> enemyDynamicBulletYIncreases = new List<int>(new int[] { });//list for dynamic type bullet Ys

        List<int> enemyHeavyBulletXValues = new List<int>(new int[] { });//list for heavy type enemy bullet Xes
        List<int> enemyHeavyBulletYValues = new List<int>(new int[] { });//list for heavy type bullet Ys

        List<int> powerupXValues = new List<int>(new int[] { });//list for powerup xes
        List<int> powerupYValues = new List<int>(new int[] { });//list for powerup ys

        List<int> explosionXValues = new List<int>(new int[] { });//list for explosion xes
        List<int> explosionYValues = new List<int>(new int[] { });//list for explosion ys
        List<int> explosionSizeValues = new List<int>(new int[] { });//list for explosion sizes
        List<int> explosionOpacityValues = new List<int>(new int[] { });//list for explosion opacities


        Boolean leftArrowDown, downArrowDown, rightArrowDown, upArrowDown, spaceDown, jDown, kDown, lDown, iDown, mDown;//track whether keys are held down

        public Form1()
        {

            ///loads all sounds to prevent lag on first few plays
            p2ShotPlayer.Open(new System.Uri(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "laserShot.mp3")));
            shotPlayer.Open(new System.Uri(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "laserShot.mp3")));
            collectPlayer.Open(new System.Uri(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "collectSound.wav")));
            boomPlayer.Open(new System.Uri(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "grenadeSound.wav")));
            hitPlayer.Open(new System.Uri(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "boom.wav")));
            bgMusicPlayer.Open(new System.Uri(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "bgMusic.mp3")));

            bgMusicPlayer.Play();// loops bg music


            SplashScreen();//calls splaschreen method
            InitializeComponent();


            playerX = this.Width / 2;//initializes player to midcsreen
            player2X = this.Width / 2;

            playerY = this.Height - 100;

            entryBox.Visible = false;
            enterNameButton.Visible = false;


            for (int i = 0; i < 199; i++)//randomises star locations and sizes
            {
                starXValues[i] = rand.Next(0, this.Width - 1);
                starYValues[i] = rand.Next(0, this.Height - 1);
                starSizeValues[i] = rand.Next(3, 6);
            }

            musicWatch.Start();//starts music stopwatch
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (gameState == "one player")
            {
                //check to see if a key is pressed and set is KeyDown value to true if it has
                switch (e.KeyCode)
                {
                    case Keys.W:
                        upArrowDown = true;
                        break;
                    case Keys.A:
                        leftArrowDown = true;
                        break;
                    case Keys.S:
                        downArrowDown = true;
                        break;
                    case Keys.D:
                        rightArrowDown = true;
                        break;
                    case Keys.Space:
                        spaceDown = true;
                        break;
                    case Keys.P://pause case
                        if (gamePaused == false && gameOver == false)
                        {
                            gamePaused = true;
                            gameTimer.Enabled = false;
                            Refresh();
                        }
                        else
                        {
                            gamePaused = false;
                            gameTimer.Enabled = true;
                        }
                        break;
                    //returns to title from pause screen
                    case Keys.Escape:
                        if (gamePaused)
                        {
                            quit();
                        }
                        break;
                    default:
                        break;
                }
            }
            //TWO PLAYER CASE
            else if (gameState == "two player")
            {
                switch (e.KeyCode)
                {
                    case Keys.W:
                        upArrowDown = true;
                        break;
                    case Keys.A:
                        leftArrowDown = true;
                        break;
                    case Keys.S:
                        downArrowDown = true;
                        break;
                    case Keys.D:
                        rightArrowDown = true;
                        break;
                    case Keys.Space:
                        spaceDown = true;
                        break;
                    case Keys.J:
                        jDown = true;
                        break;
                    case Keys.K:
                        kDown = true;
                        break;
                    case Keys.L:
                        lDown = true;
                        break;
                    case Keys.I:
                        iDown = true;
                        break;
                    case Keys.M:
                        mDown = true;
                        break;
                    case Keys.P:
                        if (gamePaused == false && gameOver == false)
                        {   //pauses game
                            gamePaused = true;
                            gameTimer.Enabled = false;
                            Refresh();
                        }
                        else
                        {   //unpauses game
                            gamePaused = false;
                            gameTimer.Enabled = true;
                        }
                        break;
                    //returns to title from pause screen
                    case Keys.Escape:
                        if (gamePaused)
                        {
                            quit();
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        //*****************KEYUP METHOD*****************************
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (gameState == "one player")
            {
                //check to see if a key has been released and set its KeyDown value to false if it has
                switch (e.KeyCode)
                {
                    case Keys.A:
                        leftArrowDown = false;
                        break;
                    case Keys.S:
                        downArrowDown = false;
                        break;
                    case Keys.D:
                        rightArrowDown = false;
                        break;
                    case Keys.W:
                        upArrowDown = false;
                        break;
                    case Keys.Space:
                        spaceDown = false;
                        break;
                    default:
                        break;
                }              
            }

            else if (gameState == "two player")
            {
                switch (e.KeyCode)
                {
                    case Keys.A:
                        leftArrowDown = false;
                        break;
                    case Keys.S:
                        downArrowDown = false;
                        break;
                    case Keys.D:
                        rightArrowDown = false;
                        break;
                    case Keys.W:
                        upArrowDown = false;
                        break;
                    case Keys.Space:
                        spaceDown = false;
                        break;
                    case Keys.J:
                        jDown = false;
                        break;
                    case Keys.K:
                        kDown = false;
                        break;
                    case Keys.L:
                        lDown = false;
                        break;
                    case Keys.I:
                        iDown = false;
                        break;
                    case Keys.M:
                        mDown = false;
                        break;
                    default:
                        break;
                }
            }
        }

        //timer tick method
        private void timer1_Tick(object sender, EventArgs e)
        {

            if (gameState == "one player")
            {
                //adjusts player speed
                if (playerFast) { playerSpeed = 12; }
                else { playerSpeed = 6; }

                //******************PLAYER SHOT SPAWNS*************************************************
                if (spaceDown == true && bulletModulator == 10 && playerOk)//fires shots only if bulletModulator has reached 10
                {
                    playerFiring = true;
                    switch (fireMode)//determines which type of bullet to add based on current firing mode. This is changed via randomly appearing powerups.
                    {
                        case "single": //fires a single shot. Start case.
                            bulletXValues.Add(playerX + 24);
                            bulletYValues.Add(playerY);
                            bulletTypeValues.Add("light");
                            bulletModulator = 0;
                            break;
                        case "double": //fires two parallel shots. Shots same as in single
                            bulletXValues.Add(playerX + 4);
                            bulletYValues.Add(playerY);
                            bulletTypeValues.Add("light");
                            bulletXValues.Add(playerX + 44);
                            bulletYValues.Add(playerY);
                            bulletTypeValues.Add("light");
                            bulletModulator = 0;
                            break;
                        case "spread"://fires three shots that spread out
                            bulletXValues.Add(playerX + 4);
                            bulletYValues.Add(playerY);
                            bulletTypeValues.Add("spread left");
                            bulletXValues.Add(playerX + 24);
                            bulletYValues.Add(playerY);
                            bulletTypeValues.Add("spread center");
                            bulletXValues.Add(playerX + 44);
                            bulletYValues.Add(playerY);
                            bulletTypeValues.Add("spread right");
                            bulletModulator = 0;
                            break;
                        case "heavy": //fires a single heavy shot.
                            bulletXValues.Add(playerX + 20);
                            bulletYValues.Add(playerY - 15);
                            bulletTypeValues.Add("heavy");
                            bulletModulator = 0;
                            break;
                        default:
                            break;
                    }

                    shotPlayer.Open(new System.Uri(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "laserShot.mp3")));//sets pewplayer's sound
                    shotPlayer.Play();//plays shot sound
                }

                else if (bulletModulator > 2) { playerFiring = false; }

                moveP1();

                if (bulletModulator < 10) { bulletModulator++; }//cases bullet modulator to incement if a shot is not ready. This will cause a shot to be fired every 100 ms

                //******************BULLETS************************************************
                for (int i = 0; i < bulletXValues.Count(); i++)
                {
                    //causes bullets to ascend
                    if (bulletYValues[i] < 0)
                    {
                        removeBullets(i);
                    }
                    else
                    {
                        switch (bulletTypeValues[i])
                        {//switch to determine how player bullets will move
                            case "light":
                                bulletYValues[i] -= 10;
                                break;
                            case "heavy":
                                bulletYValues[i] -= 5;
                                break;
                            case "spread left":
                                bulletYValues[i] -= 10;
                                bulletXValues[i] -= 5;
                                break;
                            case "spread center":
                                bulletYValues[i] -= 10;
                                break;
                            case "spread right":
                                bulletYValues[i] -= 10;
                                bulletXValues[i] += 5;
                                break;
                        }
                    }
                    for (int j = 0; j < enemyXValues.Count(); j++)//detects collisions with enemies
                    {
                        bool enemyHit = false;//tracks whether the enemy is hit to allow for the enemy switch to not impact the shot switch with less code.  
                        try
                        {
                            if (enemyXValues.IndexOf(enemyXValues[j]) != -1 && bulletXValues.IndexOf(bulletXValues[i]) != -1)
                            {
                                switch (enemyTypeValues[j])//determines hitbox based on enemy type
                                {
                                    //uses distance formula to check for collision
                                    case "light":
                                        if (calculateDistance(bulletXValues[i], enemyXValues[j], bulletYValues[i], enemyYValues[j]) < 30)
                                        {
                                            enemyHit = true;
                                        }
                                        break;
                                    case "dynamic":
                                        if (calculateDistance(bulletXValues[i], enemyXValues[j], bulletYValues[i], enemyYValues[j]) < 60)
                                        {
                                            enemyHit = true;
                                        }
                                        break;
                                    case "heavy":
                                        if (calculateDistance(bulletXValues[i], enemyXValues[j], bulletYValues[i], enemyYValues[j]) < 80)
                                        {
                                            enemyHit = true;
                                        }
                                        break;
                                }
                            }
                        }
                        catch { }
                        if (enemyHit)
                        {
                            switch (bulletTypeValues[i])//varies damage based on shot type
                            {
                                case "light":
                                    enemyHealths[j] -= 2;
                                    removeBullets(i);
                                    break;
                                case "heavy":
                                    enemyHealths[j] -= 3;
                                    removeBullets(i);
                                    break;
                                default: //covers spread shots
                                    enemyHealths[j] -= 1;
                                    removeBullets(i);
                                    break;
                            }

                            hitPlayer.Open(new System.Uri(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "boom.wav")));//sets boomplayer's sound
                            hitPlayer.Play();//plays hit sound
                        }
                    }
                }

                for (int i = 0; i < 199; i++)
                {
                    starYValues[i] += starSizeValues[i] * 0.05;
                    if (starYValues[i] > this.Height) { starYValues[i] = 0; }//causes stars to "snake" back up to top when they go offscreen.
                }

                //******************ENEMIES*************************************************
                for (int i = 0; i < enemyXValues.Count(); i++)
                {
                    if (enemyYValues[i] > this.Height)//removes offscreen enemies
                    {
                        removeEnemies(i);
                    }
                    else
                    {
                        int scoreMod = 0;//tracks the amount to add to the score on an enemy kill
                                         //tracks the amount to increase x and y by in an explosion to centralise explosion
                        int explosionXMod = 0;
                        int explosionYMod = 0;

                        //switch for movement pattern based on enemy type
                        switch (enemyTypeValues[i])
                        {
                            case "light":
                                enemyYValues[i] += 2;//moves light enemies down
                                scoreMod = 50;
                                explosionXMod = 15;
                                explosionYMod = 25;
                                break;
                            case "dynamic":
                                enemyYValues[i] += 3;//moves dynamic enemies down
                                enemyXValues[i] = enemyStartXes[i] + Convert.ToInt16(100 * Math.Sin(0.01 * enemyYValues[i]));//causes dynamic enemies to sway in a sinusoidal wave
                                scoreMod = 100;
                                explosionXMod = 30;
                                explosionYMod = 20;
                                break;
                            case "heavy":
                                enemyYValues[i] += 1;//causes heavy enemies to slowly move down
                                scoreMod = 150;
                                explosionXMod = 30;
                                explosionYMod = 40;
                                break;
                        }

                        //kills player and enemy on collision
                        if (calculateDistance(enemyXValues[i], playerX, enemyYValues[i], playerY) < 40)
                        {
                            if (playerInvincible == false) { playerHealth = 0; }

                            enemyHealths[i] = 0;
                        }

                        if (enemyHealths[i] <= 0)
                        {
                            //adds explosion to defeated enemies
                            explosionXValues.Add(enemyXValues[i] + explosionXMod);
                            explosionYValues.Add(enemyYValues[i] + explosionYMod);
                            explosionSizeValues.Add(0);
                            explosionOpacityValues.Add(0);

                            boomPlayer.Open(new System.Uri(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "grenadeSound.wav")));//sets boomplayer's sound
                            boomPlayer.Play();//plays boom sound

                            removeEnemies(i);
                            score += scoreMod;
                        }
                    }
                }

                //******************REGULAR ENEMY BULLETS*************************************************
                for (int i = 0; i < enemyBulletXValues.Count(); i++)
                {
                    if (enemyBulletYValues[i] < 0)
                    {
                        removeEnemyBullets(i);
                    }
                    else
                    {
                        enemyBulletYValues[i] += 7;//causes enemy bullets to descend
                        if (calculateDistance(playerX + 30, enemyBulletXValues[i], playerY, enemyBulletYValues[i]) < 30 && playerInvincible == false)
                        {
                            playerHealth -= 1;
                            removeEnemyBullets(i);

                            hitPlayer.Open(new System.Uri(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "boom.wav")));//sets boomplayer's sound
                            hitPlayer.Play();//plays hit sound
                        }
                    }
                }

                //******************DYNAMIC ENEMY BULLETS*************************************************
                for (int i = 0; i < enemyDynamicBulletXValues.Count(); i++)//for loop for dynamic bullets. Seperate due to their differet behavior
                {
                    if (enemyDynamicBulletYValues[i] < 0)
                    {
                        removeDynamicBullets(i);
                    }
                    else
                    {
                        //moves enemy shots as determined by the player's position during their creation.
                        enemyDynamicBulletXValues[i] += enemyDynamicBulletXIncreases[i];
                        enemyDynamicBulletYValues[i] += enemyDynamicBulletYIncreases[i];

                        if (enemyDynamicBulletYIncreases[i] < 5) { enemyDynamicBulletYIncreases[i] = 5; }//sets minimum vertical movement
                        if (enemyDynamicBulletXIncreases[i] < -5) { enemyDynamicBulletXIncreases[i] = -5; }//sets max horizontal movement
                        if (enemyDynamicBulletXIncreases[i] > 5) { enemyDynamicBulletXIncreases[i] = 5; }//sets max horizontal movement
                    }
                    try
                    {
                        if (calculateDistance(playerX + 30, enemyDynamicBulletXValues[i], playerY, enemyDynamicBulletYValues[i]) < 30 && playerInvincible == false)
                        {
                            playerHealth -= 2;
                            removeDynamicBullets(i);

                            hitPlayer.Open(new System.Uri(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "boom.wav")));//sets boomplayer's sound
                            hitPlayer.Play();//plays hit sound
                        }
                    }
                    catch { }
                }

                //******************HEAVY ENEMY BULLETS*************************************************
                for (int i = 0; i < enemyHeavyBulletXValues.Count(); i++)
                {
                    if (enemyHeavyBulletYValues[i] > this.Height)
                    {
                        removeHeavyBullets(i);
                    }
                    else
                    {
                        //moves enemy heavy shots down
                        enemyHeavyBulletYValues[i] += 4;
                        //causes heavy shots to "home" towards player
                        if (enemyHeavyBulletXValues[i] > playerX) { enemyHeavyBulletXValues[i]--; }
                        else if (enemyHeavyBulletXValues[i] < playerX) { enemyHeavyBulletXValues[i]++; }
                        try
                        {      //colliion detection with player
                            if (calculateDistance(playerX + 30, enemyHeavyBulletXValues[i], playerY, enemyHeavyBulletYValues[i]) < 30 && playerInvincible == false)
                            {
                                playerHealth -= 3;
                                removeHeavyBullets(i);

                                hitPlayer.Open(new System.Uri(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "boom.wav")));//sets boomplayer's sound
                                hitPlayer.Play();//plays hit sound
                            }
                        }
                        catch { }
                    }
                }

                //***************PLAYER RESPAWN***************
                if (playerHealth <= 0 && gameOver == false)
                {
                    if (playerDying == false)
                    {
                        explosionXValues.Add(playerX + 30);
                        explosionYValues.Add(playerY + 20);
                        explosionSizeValues.Add(0);
                        explosionOpacityValues.Add(0);
                        playerOk = false;
                        playerDying = true;

                        boomPlayer.Open(new System.Uri(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "grenadeSound.wav")));//sets boomplayer's sound
                        boomPlayer.Play();//plays boom sound
                    }

                    //waits for explosion to finish before stopping to respawn player
                    if (explosionXValues.Count() == 0)
                    {

                        gameTimer.Enabled = false;

                        for (int i = 0; i < 5; i++)//flashes player onscreen 5 times before restarting
                        {
                            if (playerLives != 1)
                            {
                                Refresh();
                                Thread.Sleep(250);
                                playerOk = false;
                                Refresh();
                                Thread.Sleep(250);
                                playerOk = true;
                                playerDying = false;
                            }

                        }
                        //respawns player with fresh stats
                        gameTimer.Enabled = true;
                        fireMode = "single";
                        playerInvincible = false;
                        playerFast = false;
                        playerLives--;
                        playerHealth = 5;

                    }
                }

                if (playerLives == 0)//ends game on player death
                {
                    playerOk = false;
                    Refresh();
                    gameTimer.Enabled = false;

                    gameOver = true;

                    entryBox.Visible = true;
                    enterNameButton.Visible = true;

                }

                int powerupRand = rand.Next(0, 501);//1 in 500 chance of a powerup spawn

                if (powerupRand == 500 && timeSincePowerup > 300)//only spawns powerup if random is 500 and there have been 300 frames since last spawn
                {
                    //starts powerup at a random x at the top of the screen
                    powerupXValues.Add(rand.Next(0, this.Width - 50));
                    powerupYValues.Add(0);
                    timeSincePowerup = 0;
                }
                else { timeSincePowerup++; }

                //******************POWERUPS*************************************************
                for (int i = 0; i < powerupXValues.Count(); i++)
                {
                    if (powerupYValues[i] > this.Height)//removes offscreen powerups
                    {
                        removePowerups(i);
                    }
                    else
                    {
                        powerupYValues[i]++;//moves powerup downn
                        if (calculateDistance(playerX, powerupXValues[i], playerY, powerupYValues[i]) < 50)
                        {
                            int randomEffect = rand.Next(1, 6);
                            switch (randomEffect)
                            {
                                case 1:
                                    fireMode = "double";
                                    break;
                                case 2:
                                    fireMode = "spread";
                                    break;
                                case 3:
                                    fireMode = "heavy";
                                    break;
                                case 4:
                                    invincibilityWatch.Start();
                                    playerInvincible = true;
                                    break;
                                case 5:
                                    speedWatch.Start();
                                    playerFast = true;
                                    break;
                            }

                            removePowerups(i);

                            collectPlayer.Open(new System.Uri(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "collectSound.wav")));//sets collectplayer's sound
                            collectPlayer.Play();//plays collect sound                      
                        }
                    }
                }

                //makes player vulnerable if they have been invincible for more than 15 seconds
                if (playerInvincible && invincibilityWatch.Elapsed.TotalHours >= 0.00416)//approx 15 secs invincibility
                {
                    invincibilityWatch.Stop();
                    invincibilityWatch.Reset();
                    playerInvincible = false;
                }

                //makes player normal if they have been fast for more than 15 seconds
                if (playerFast && speedWatch.Elapsed.TotalHours >= 0.00632)//approx 22 secs speed
                {
                    speedWatch.Stop();
                    speedWatch.Reset();
                    playerFast = false;
                }
                //******************EXPLOSIONS*************************************************
                handleExplosions();

                //******************ENEMY SPAWNING*************************************************
                if (tracker % 200 == 0)
                {
                    //enemies spawn every 200 frames
                    if (tracker <= 800)
                    {
                        double screenDiv = this.Width / enemySpawnRate;//used to evenly distribute enemies across screen
                        for (int i = 0; i < enemySpawnRate; i += 1)
                        {
                            int startX = (i * Convert.ToInt16(screenDiv) + Convert.ToInt16(screenDiv) / 2 - 30);
                            enemyXValues.Add(startX);
                            enemyYValues.Add(-50 + (rand.Next(-50, 51)));//randomises Y to a degree
                            enemyTypeValues.Add("light");
                            enemyHealths.Add(1);
                            enemyStartXes.Add(startX);
                        }
                        enemySpawnRate += 1;
                        if (enemySpawnRate == 6) { enemySpawnRate = 0; } //resets spawn rate once next bracket is reached
                    }
                    else if (tracker > 800 && tracker <= 1800)
                    {
                        double screenDiv = this.Width / enemySpawnRate;//used to evenly distribute enemies across screen
                        for (int i = 0; i < enemySpawnRate; i += 1)
                        {
                            int startX = (i * Convert.ToInt16(screenDiv) + Convert.ToInt16(screenDiv) / 2 - 30);
                            enemyXValues.Add(startX);
                            enemyYValues.Add(-50 + (rand.Next(-50, 51)));//randomises Y to a degree
                            enemyTypeValues.Add("dynamic");
                            enemyHealths.Add(3);
                            enemyStartXes.Add(startX);//used to allow a sine wave pattern.
                            if (enemySpawnRate == 5) { enemySpawnRate = 0; }
                        }

                        enemySpawnRate += 1;
                    }

                    else if (tracker > 2000 && tracker <= 3000)
                    {
                        double screenDiv = this.Width / enemySpawnRate;//used to evenly distribute enemies across screen
                        for (int i = 0; i < enemySpawnRate; i += 1)
                        {
                            int startX = (i * Convert.ToInt16(screenDiv) + Convert.ToInt16(screenDiv) / 2 - 30);

                            enemyXValues.Add(startX);
                            enemyYValues.Add(-50 + (rand.Next(-50, 51)));//randomises Y within 100 pixels
                            enemyTypeValues.Add("heavy");
                            enemyHealths.Add(5);
                            enemyStartXes.Add(startX);
                            if (enemySpawnRate == 5) { enemySpawnRate = 0; }
                        }

                        enemySpawnRate += 1;
                    }

                    else if (tracker > 3000 && tracker <= 5000)
                    {
                        double screenDiv = this.Width / enemySpawnRate;//used to evenly distribute enemies across screen
                        for (int i = 0; i < enemySpawnRate; i += 1)
                        {
                            int startX = (i * Convert.ToInt16(screenDiv) + Convert.ToInt16(screenDiv) / 2 - 30);

                            enemyXValues.Add(startX);
                            enemyYValues.Add(-50 + (rand.Next(-50, 51)));//randomises Y within 100 pixels

                            int typeRand = rand.Next(0, 2);//randomises enemy type

                            //determines type to spawn
                            switch (typeRand)
                            {
                                case 0:
                                    enemyTypeValues.Add("light");
                                    enemyHealths.Add(1);
                                    break;
                                case 1:
                                    enemyTypeValues.Add("dynamic");
                                    enemyHealths.Add(3);
                                    break;
                            }

                            enemyStartXes.Add(startX);
                            if (enemySpawnRate == 7) { enemySpawnRate = 5; }
                        }

                        enemySpawnRate += 1;
                    }
                    else if (tracker > 5000 && tracker <= 7000)
                    {
                        double screenDiv = this.Width / enemySpawnRate;//used to evenly distribute enemies across screen
                        for (int i = 0; i < enemySpawnRate; i += 1)
                        {
                            int startX = (i * Convert.ToInt16(screenDiv) + Convert.ToInt16(screenDiv) / 2 - 30);

                            enemyXValues.Add(startX);
                            enemyYValues.Add(-50 + (rand.Next(-50, 51)));//randomises Y within 100 pixels

                            int typeRand = rand.Next(0, 2);//randomises enemy type

                            //determines type to spawn
                            switch (typeRand)
                            {
                                case 0:
                                    enemyTypeValues.Add("light");
                                    enemyHealths.Add(1);
                                    break;
                                case 1:
                                    enemyTypeValues.Add("heavy");
                                    enemyHealths.Add(5);
                                    break;
                            }

                            enemyStartXes.Add(startX);

                            if (enemySpawnRate == 8) { enemySpawnRate = 6; }
                        }

                        enemySpawnRate += 1;
                    }
                    else if (tracker > 7000 && tracker <= 9000)
                    {
                        double screenDiv = this.Width / enemySpawnRate;//used to evenly distribute enemies across screen
                        for (int i = 0; i < enemySpawnRate; i += 1)
                        {
                            int startX = (i * Convert.ToInt16(screenDiv) + Convert.ToInt16(screenDiv) / 2 - 30);

                            enemyXValues.Add(startX);
                            enemyYValues.Add(-50 + (rand.Next(-50, 51)));//randomises Y within 100 pixels

                            int typeRand = rand.Next(0, 2);//randomises enemy type

                            //determines type to spawn
                            switch (typeRand)
                            {
                                case 0:
                                    enemyTypeValues.Add("dynamic");
                                    enemyHealths.Add(3);
                                    break;
                                case 1:
                                    enemyTypeValues.Add("heavy");
                                    enemyHealths.Add(1);
                                    break;
                            }
                            enemyStartXes.Add(startX);
                            if (enemySpawnRate == 9) { enemySpawnRate = 7; }
                        }
                        enemySpawnRate += 1;
                    }
                    else if (tracker > 9000)
                    {
                        double screenDiv = this.Width / enemySpawnRate;//used to evenly distribute enemies across screen
                        for (int i = 0; i < enemySpawnRate; i += 1)
                        {
                            int startX = (i * Convert.ToInt16(screenDiv) + Convert.ToInt16(screenDiv) / 2 - 30);

                            enemyXValues.Add(startX);
                            enemyYValues.Add(-50 + (rand.Next(-50, 51)));//randomises Y within 100 pixels

                            int typeRand = rand.Next(0, 3);//randomises enemy type

                            //determines type to spawn
                            switch (typeRand)
                            {
                                case 0:
                                    enemyTypeValues.Add("dynamic");
                                    enemyHealths.Add(3);
                                    break;
                                case 1:
                                    enemyTypeValues.Add("heavy");
                                    enemyHealths.Add(5);
                                    break;
                                case 2:
                                    enemyTypeValues.Add("light");
                                    enemyHealths.Add(1);
                                    break;
                            }

                            enemyStartXes.Add(startX);
                        }

                        enemySpawnRate += 1;
                    }
                }
                //******************ENEMY FIRE PATTERNS*************************************************

                //light bullet spawns
                if (tracker % 50 == 0)
                {
                    for (int i = 0; i < enemyXValues.Count(); i++)
                    {
                        if (enemyTypeValues[i] == "light")
                        {
                            enemyBulletXValues.Add(enemyXValues[i] + 15);
                            enemyBulletYValues.Add(enemyYValues[i] + 55);
                        }
                    }
                }

                //dynamic bullet spawns
                if (tracker % 35 == 0)
                {
                    for (int i = 0; i < enemyXValues.Count(); i++)
                    {
                        if (enemyTypeValues[i] == "dynamic")
                        {
                            enemyDynamicBulletXValues.Add(enemyXValues[i] + 15);
                            enemyDynamicBulletYValues.Add(enemyYValues[i] + 55);

                            //sends shot towards the player
                            enemyDynamicBulletXIncreases.Add((playerX - enemyXValues[i]) / 50);
                            enemyDynamicBulletYIncreases.Add((playerY - enemyYValues[i]) / 50);


                        }
                    }
                }

                //heavy bullet spawns
                if (tracker % 65 == 0)
                {
                    for (int i = 0; i < enemyXValues.Count(); i++)
                    {
                        if (enemyTypeValues[i] == "heavy")
                        {
                            enemyHeavyBulletXValues.Add(enemyXValues[i] + 5);
                            enemyHeavyBulletYValues.Add(enemyYValues[i] + 80);

                            enemyHeavyBulletXValues.Add(enemyXValues[i] + 55);
                            enemyHeavyBulletYValues.Add(enemyYValues[i] + 80);


                        }
                    }
                }

                checkMusic();

                tracker++;
                Refresh();
            }
            else if (gameState == "two player")
            //**************P1 MOVEMENT*******************
            {
                moveP1();
                //*********************P2 MOVEMENT************************
                moveP2();

                //******************PLAYER SHOT SPAWNS*************************************************
                if (spaceDown == true && bulletModulator == 10 && playerOk)//fires shots only if bulletModulator has reached 10
                {
                    playerFiring = true;
                    switch (fireMode)//determines which type of bullet to add based on current firing mode. This is changed via randomly appearing powerups.
                    {
                        case "single": //fires a single shot. Start case.
                            bulletXValues.Add(playerX + 24);
                            bulletYValues.Add(playerY);
                            bulletTypeValues.Add("light");
                            bulletModulator = 0;
                            break;
                        case "double": //fires two parallel shots. Shots same as in single
                            bulletXValues.Add(playerX + 4);
                            bulletYValues.Add(playerY);
                            bulletTypeValues.Add("light");
                            bulletXValues.Add(playerX + 44);
                            bulletYValues.Add(playerY);
                            bulletTypeValues.Add("light");
                            bulletModulator = 0;
                            break;
                        case "spread"://fires three shots that spread out
                            bulletXValues.Add(playerX + 4);
                            bulletYValues.Add(playerY);
                            bulletTypeValues.Add("spread left");
                            bulletXValues.Add(playerX + 24);
                            bulletYValues.Add(playerY);
                            bulletTypeValues.Add("spread center");
                            bulletXValues.Add(playerX + 44);
                            bulletYValues.Add(playerY);
                            bulletTypeValues.Add("spread right");
                            bulletModulator = 0;
                            break;
                        case "heavy": //fires a single heavy shot.
                            bulletXValues.Add(playerX + 20);
                            bulletYValues.Add(playerY - 15);
                            bulletTypeValues.Add("heavy");
                            bulletModulator = 0;
                            break;
                        default:
                            break;
                    }
                    shotPlayer.Open(new System.Uri(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "laserShot.mp3")));//sets pewplayer's sound
                    shotPlayer.Play();//plays shot sound
                }
                else if (bulletModulator > 2)
                {
                    playerFiring = false;
                }
                if (bulletModulator < 10) { bulletModulator++; }

                if (mDown == true && p2BulletModulator == 10 && player2Ok)//fires shots only if bulletModulator has reached 10
                {
                    player2Firing = true;
                    switch (p2FireMode)//determines which type of bullet to add based on current firing mode. This is changed via randomly appearing powerups.
                    {
                        case "single": //fires a single shot. Start case.
                            p2BulletXValues.Add(player2X + 24);
                            p2BulletYValues.Add(player2Y + 45);
                            p2BulletTypeValues.Add("light");
                            p2BulletModulator = 0;
                            break;
                        case "double": //fires two parallel shots. Shots same as in single
                            p2BulletXValues.Add(player2X + 4);
                            p2BulletYValues.Add(player2Y + 35);
                            p2BulletTypeValues.Add("light");
                            p2BulletXValues.Add(player2X + 44);
                            p2BulletYValues.Add(player2Y + 35);
                            p2BulletTypeValues.Add("light");
                            p2BulletModulator = 0;
                            break;
                        case "spread"://fires three shots that spread out
                            p2BulletXValues.Add(player2X + 4);
                            p2BulletYValues.Add(player2Y + 35);
                            p2BulletTypeValues.Add("spread left");
                            p2BulletXValues.Add(player2X + 24);
                            p2BulletYValues.Add(player2Y + 45);
                            p2BulletTypeValues.Add("spread center");
                            p2BulletXValues.Add(player2X + 44);
                            p2BulletYValues.Add(player2Y + 35);
                            p2BulletTypeValues.Add("spread right");
                            p2BulletModulator = 0;
                            break;
                        case "heavy": //fires a single heavy shot.
                            p2BulletXValues.Add(player2X + 20);
                            p2BulletYValues.Add(player2Y + 45);
                            p2BulletTypeValues.Add("heavy");
                            p2BulletModulator = 0;
                            break;
                        default:
                            break;
                    }
                    p2ShotPlayer.Open(new System.Uri(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "laserShot.mp3")));//sets pewplayer's sound
                    p2ShotPlayer.Play();//plays shot sound
                }
                else if (p2BulletModulator > 2)
                {
                    player2Firing = false;
                }
                if (p2BulletModulator < 10) { p2BulletModulator++; }

                //*************PLAYER 1 BULLETS******************
                for (int i = 0; i < bulletXValues.Count(); i++)
                {
                    if (bulletYValues[i] < 0)//removes offscreen shots
                    {
                        removeBullets(i);
                    }
                    else
                    {
                        switch (bulletTypeValues[i])
                        {//switch to determine how player bullets will move
                            case "light":
                                bulletYValues[i] -= 10;
                                break;
                            case "heavy":
                                bulletYValues[i] -= 5;
                                break;
                            case "spread left":
                                bulletYValues[i] -= 10;
                                bulletXValues[i] -= 5;
                                break;
                            case "spread center":
                                bulletYValues[i] -= 10;
                                break;
                            case "spread right":
                                bulletYValues[i] -= 10;
                                bulletXValues[i] += 5;
                                break;
                        }

                        if (calculateDistance(bulletXValues[i], (player2X + 30), bulletYValues[i], player2Y) < 30)
                        {
                            switch (bulletTypeValues[i])//varies damage based on sht type
                            {
                                case "light":
                                    player2Health -= 2;
                                    removeBullets(i);
                                    break;
                                case "heavy":
                                    player2Health -= 3;
                                    removeBullets(i);
                                    break;
                                default: //covers spread shots
                                    player2Health -= 1;
                                    removeBullets(i);
                                    break;
                            }
                            hitPlayer.Open(new System.Uri(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "boom.wav")));//sets boomplayer's sound
                            hitPlayer.Play();//plays hit sound
                        }
                    }
                }

                //*************PLAYER 2 BULLETS******************
                for (int i = 0; i < p2BulletXValues.Count(); i++)
                {
                    //removes offscreen shots
                    if (p2BulletYValues[i] < 0)
                    {
                        removeP2Bullets(i);
                    }
                    else
                    {
                        switch (p2BulletTypeValues[i])
                        {//switch to determine how player bullets will move
                            case "light":
                                p2BulletYValues[i] += 10;
                                break;
                            case "heavy":
                                p2BulletYValues[i] += 5;
                                break;
                            case "spread left":
                                p2BulletYValues[i] += 10;
                                p2BulletXValues[i] -= 5;
                                break;
                            case "spread center":
                                p2BulletYValues[i] += 10;
                                break;
                            case "spread right":
                                p2BulletYValues[i] += 10;
                                p2BulletXValues[i] += 5;
                                break;
                        }
                        if (calculateDistance(p2BulletXValues[i], (playerX + 30), p2BulletYValues[i], playerY) < 30)
                        {
                            //varies damage based on shot type
                            switch (p2BulletTypeValues[i])
                            {
                                case "light":
                                    playerHealth -= 2;
                                    removeP2Bullets(i);
                                    break;
                                case "heavy":
                                    playerHealth -= 3;
                                    removeP2Bullets(i);
                                    break;
                                default:
                                    //covers spread shots
                                    playerHealth -= 1;
                                    removeP2Bullets(i);
                                    break;
                            }

                            hitPlayer.Open(new System.Uri(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "boom.wav")));//sets boomplayer's sound
                            hitPlayer.Play();//plays hit sound
                        }
                    }
                }

                //************PLAYER DEATHS**************
                if (playerHealth <= 0 && gameOver == false)//player respawn
                {
                    if (playerDying == false)
                    {
                        explosionXValues.Add(playerX + 30);
                        explosionYValues.Add(playerY + 20);
                        explosionSizeValues.Add(0);
                        explosionOpacityValues.Add(0);
                        playerOk = false;
                        playerDying = true;

                        boomPlayer.Open(new System.Uri(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "grenadeSound.wav")));//sets boomplayer's sound
                        boomPlayer.Play();//plays boom sound
                    }

                    //waits for explosion to finish before stopping to respawn player
                    if (explosionXValues.Count() == 0)
                    {
                        gameTimer.Enabled = false;


                        //flashes player onscreen 5 times before restarting
                        for (int i = 0; i < 5; i++)
                        {
                            if (playerLives != 1)
                            {
                                Refresh();
                                Thread.Sleep(250);
                                playerOk = false;
                                Refresh();
                                Thread.Sleep(250);
                                playerOk = true;
                                playerDying = false;
                            }

                        }
                        //respawns player with fresh stats
                        gameTimer.Enabled = true;
                        fireMode = "single";
                        playerLives--;
                        playerHealth = 5;

                    }
                }

                ///p2 respawn
                if (player2Health <= 0 && gameOver == false)
                {
                    if (player2Dying == false)
                    {
                        explosionXValues.Add(player2X + 30);
                        explosionYValues.Add(player2Y + 20);
                        explosionSizeValues.Add(0);
                        explosionOpacityValues.Add(0);
                        player2Ok = false;
                        player2Dying = true;

                        boomPlayer.Open(new System.Uri(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "grenadeSound.wav")));//sets boomplayer's sound
                        boomPlayer.Play();//plays boom sound
                    }

                    //waits for explosion to finish before stopping to respawn player
                    if (explosionXValues.Count() == 0)
                    {

                        gameTimer.Enabled = false;

                        for (int i = 0; i < 5; i++)//flashes player 2 onscreen 5 times before restarting
                        {
                            if (player2Lives != 1)
                            {
                                Refresh();
                                Thread.Sleep(250);
                                player2Ok = false;
                                Refresh();
                                Thread.Sleep(250);
                                player2Ok = true;
                                player2Dying = false;
                            }

                        }
                        //respawns player 2 with fresh stats
                        gameTimer.Enabled = true;
                        p2FireMode = "single";
                        player2Lives--;
                        player2Health = 5;

                    }
                }
                if (playerLives == 0)//ends game on player 1 death
                {
                    playerOk = false;
                    Refresh();
                    gameTimer.Enabled = false;

                    //displays win message for 5 s and then exits to title
                    gameOver = true;
                    winner = "Player 2";
                    Refresh();
                    Thread.Sleep(5000);
                    Refresh();
                    quit();
                }
                if (player2Lives == 0)//ends game on player 2 death
                {
                    player2Ok = false;
                    Refresh();
                    gameTimer.Enabled = false;

                    //displays win message for 5 s and then exits to title
                    gameOver = true;
                    winner = "Player 1";
                    Refresh();
                    Thread.Sleep(5000);
                    Refresh();
                    quit();
                }

                handleExplosions();

                int powerupRand = rand.Next(0, 501);//1 in 500 chance of a powerup spawn
                if (powerupRand == 500 && timeSincePowerup > 150)
                {
                    //starts powerup at a random y at the left of the screen
                    powerupYValues.Add(rand.Next(50, this.Height - 50));
                    powerupXValues.Add(0);
                    timeSincePowerup = 0;
                }
                else { timeSincePowerup++; }

                //******************POWERUPS*************************************************
                for (int i = 0; i < powerupXValues.Count(); i++)
                {
                    if (powerupYValues[i] > this.Height)//removes offscreen powerups
                    {
                        removePowerups(i);
                    }
                    else
                    {
                        powerupXValues[i]++;//moves powerup right
                        //p1 powerup pickup
                        if (calculateDistance(playerX - 30, powerupXValues[i], playerY, powerupYValues[i]) < 50)
                        {
                            int randomEffect = rand.Next(1, 4);
                            switch (randomEffect)
                            {
                                case 1:
                                    fireMode = "double";
                                    break;
                                case 2:
                                    fireMode = "spread";
                                    break;
                                case 3:
                                    fireMode = "heavy";
                                    break;
                            }
                            removePowerups(i);
                            collectPlayer.Open(new System.Uri(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "collectSound.wav")));//sets collectplayer's sound
                            collectPlayer.Play();//plays collect sound

                        }
                        //p2 powerup pickups
                        else if (calculateDistance(player2X - 30, powerupXValues[i], player2Y, powerupYValues[i]) < 50)
                        {
                            int randomEffect = rand.Next(1, 4);
                            switch (randomEffect)
                            {
                                case 1:
                                    p2FireMode = "double";
                                    break;
                                case 2:
                                    p2FireMode = "spread";
                                    break;
                                case 3:
                                    p2FireMode = "heavy";
                                    break;
                            }
                            removePowerups(i);
                            collectPlayer.Open(new System.Uri(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "collectSound.wav")));//sets collectplayer's sound
                            collectPlayer.Play();//plays collect sound                           
                        }
                    }
                }
                checkMusic();

                tracker++;//increments tracker
                Refresh();//calls paint method on each tick
            }

            else if (gameState == "high scores")
            {
                Refresh();
                checkMusic();
            }
            else { checkMusic(); }
        }

        //paint method
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            if (gameState == "one player")
            {
                drawBrush.Color = Color.White;

                for (int i = 0; i < 199; i++)
                {
                    float starY = Convert.ToInt16(starYValues[i]);
                    e.Graphics.FillEllipse(drawBrush, starXValues[i], starY, starSizeValues[i], starSizeValues[i]);
                }


                //draws explosions
                for (int i = 0; i < explosionXValues.Count(); i++)
                {   //explosions become translucent over time.
                    drawBrush.Color = Color.FromArgb(255 - explosionOpacityValues[i], 202 - explosionOpacityValues[i] / 1, 0);
                    e.Graphics.FillEllipse(drawBrush, explosionXValues[i], explosionYValues[i], explosionSizeValues[i], explosionSizeValues[i]);
                    drawBrush.Color = Color.FromArgb(255 - explosionOpacityValues[i], 0, 0);
                    e.Graphics.FillEllipse(drawBrush, explosionXValues[i] + explosionSizeValues[i] / 4, explosionYValues[i] + explosionSizeValues[i] / 4, explosionSizeValues[i] / 2, explosionSizeValues[i] / 2);
                }

                if (playerOk)
                {
                    drawBrush.Color = Color.White;

                    if (playerInvincible)
                    {
                        drawBrush.Color = Color.Gold;
                    }

                    Point[] triangle1Points = { new Point(playerX, playerY + 35), new Point(playerX + 5, playerY + 10), new Point(playerX + 10, playerY + 35) };//array for the points of triangle 1
                    Point[] triangle2Points = { new Point(playerX + 20, playerY + 15), new Point(playerX + 25, playerY), new Point(playerX + 30, playerY + 15) };//array for the points of triangle 2
                    Point[] triangle3Points = { new Point(playerX + 40, playerY + 35), new Point(playerX + 45, playerY + 10), new Point(playerX + 50, playerY + 35) };//array for the points of triangle 3


                    e.Graphics.FillRectangle(drawBrush, playerX, playerY + 35, 50, 10);//draws ship base
                    e.Graphics.FillRectangle(drawBrush, playerX + 20, playerY + 15, 10, 20);//draws ship spine

                    drawBrush.Color = Color.DarkRed;

                    e.Graphics.FillPolygon(drawBrush, triangle1Points);//draws left triangle
                    e.Graphics.FillPolygon(drawBrush, triangle2Points);//draws central triangle
                    e.Graphics.FillPolygon(drawBrush, triangle3Points);//draws right triangle

                    e.Graphics.FillRectangle(drawBrush, playerX + 10, playerY + 38, 30, 4);//draws ship base detail
                    e.Graphics.FillRectangle(drawBrush, playerX + 23, playerY + 20, 4, 20);//draws ship spine detail
                }

                drawBrush.Color = Color.Orange;

                if (playerFiring)//draws shooting effect
                {
                    if (fireMode == "single" || fireMode == "heavy" || fireMode == "spread")
                    {
                        e.Graphics.FillEllipse(drawBrush, playerX + 22, playerY - 15, 6, 20);
                    }
                    if (fireMode == "double" || fireMode == "spread")
                    {
                        e.Graphics.FillEllipse(drawBrush, playerX + 2, playerY - 5, 6, 20);
                        e.Graphics.FillEllipse(drawBrush, playerX + 42, playerY - 5, 6, 20);
                    }
                }

                for (int i = 0; i < bulletXValues.Count(); i++)
                {
                    switch (bulletTypeValues[i])
                    {//switch statement to determine which bullet shape to draw
                        case "light":
                            e.Graphics.FillRectangle(drawBrush, bulletXValues[i], bulletYValues[i], 3, 10);//draws player shots
                            break;
                        case "spread left":
                            e.Graphics.FillEllipse(drawBrush, bulletXValues[i], bulletYValues[i], 3, 6);//draws player shots
                            break;
                        case "spread center":
                            e.Graphics.FillEllipse(drawBrush, bulletXValues[i], bulletYValues[i], 3, 6);//draws player shots
                            break;
                        case "spread right":
                            e.Graphics.FillEllipse(drawBrush, bulletXValues[i], bulletYValues[i], 3, 6);//draws player shots
                            break;
                        case "heavy":
                            e.Graphics.FillEllipse(drawBrush, bulletXValues[i], bulletYValues[i], 10, 10);//draws player shots
                            break;
                    }
                }

                for (int i = 0; i < enemyXValues.Count(); i++)
                {
                    switch (enemyTypeValues[i])//determines which type of enemy to draw
                    {
                        case "light"://draws a light enemy
                            Point[] shipBodyPoints = { new Point(enemyXValues[i], enemyYValues[i] + 15), new Point(enemyXValues[i] + 15, enemyYValues[i] + 55), new Point(enemyXValues[i] + 30, enemyYValues[i] + 15) };//array for the points of the ship's body

                            drawBrush.Color = Color.Gray;

                            e.Graphics.FillRectangle(drawBrush, enemyXValues[i], enemyYValues[i], 10, 15);//draws left thruster
                            e.Graphics.FillRectangle(drawBrush, enemyXValues[i] + 20, enemyYValues[i], 10, 15);//draws right thruster

                            drawBrush.Color = Color.Gold;

                            e.Graphics.FillPolygon(drawBrush, shipBodyPoints);//draws ship's body
                            drawBrush.Color = Color.DarkBlue;

                            e.Graphics.FillEllipse(drawBrush, enemyXValues[i] + 10, enemyYValues[i] + 20, 10, 15);
                            break;
                        case "dynamic"://draws dynamic enemy
                            drawBrush.Color = Color.Brown;
                            e.Graphics.FillEllipse(drawBrush, enemyXValues[i], enemyYValues[i], 5, 40);//draws left "wing"
                            e.Graphics.FillRectangle(drawBrush, enemyXValues[i] + 3, enemyYValues[i] + 17, 20, 6);//draws left connector
                            e.Graphics.FillEllipse(drawBrush, enemyXValues[i] + 20, enemyYValues[i], 20, 40);//draws ships body
                            e.Graphics.FillRectangle(drawBrush, enemyXValues[i] + 37, enemyYValues[i] + 17, 20, 6);//draws right connector
                            e.Graphics.FillEllipse(drawBrush, enemyXValues[i] + 55, enemyYValues[i], 5, 40);//draws right "wing"

                            drawBrush.Color = Color.Red;
                            e.Graphics.FillEllipse(drawBrush, enemyXValues[i] + 25, enemyYValues[i] + 12, 10, 16);

                            break;
                        case "heavy":
                            drawBrush.Color = Color.White;
                            Point[] heavyShipBodyPoints = { new Point(enemyXValues[i], enemyYValues[i]), new Point(enemyXValues[i] + 15, enemyYValues[i] + 40), new Point(enemyXValues[i] + 15, enemyYValues[i] + 80), new Point(enemyXValues[i] + 45, enemyYValues[i] + 80), new Point(enemyXValues[i] + 45, enemyYValues[i] + 40), new Point(enemyXValues[i] + 60, enemyYValues[i]) };
                            Point[] triangleDetailPoints = { new Point(enemyXValues[i] + 25, enemyYValues[i] + 50), new Point(enemyXValues[i] + 30, enemyYValues[i] + 80), new Point(enemyXValues[i] + 35, enemyYValues[i] + 50) };

                            e.Graphics.FillPolygon(drawBrush, heavyShipBodyPoints);//draws ships body

                            drawBrush.Color = Color.Gray;
                            e.Graphics.FillRectangle(drawBrush, enemyXValues[i] + 15, enemyYValues[i] + 80, 5, 10);//draws left cannon
                            e.Graphics.FillRectangle(drawBrush, enemyXValues[i] + 40, enemyYValues[i] + 80, 5, 10);//draws right cannon

                            e.Graphics.FillRectangle(drawBrush, enemyXValues[i], enemyYValues[i] - 10, 10, 10);//draws left thruster
                            e.Graphics.FillRectangle(drawBrush, enemyXValues[i] + 50, enemyYValues[i] - 10, 10, 10);//draws right thruster

                            drawBrush.Color = Color.Blue;
                            e.Graphics.FillRectangle(drawBrush, enemyXValues[i] + 17, enemyYValues[i] + 40, 12, 7);//draws left window
                            e.Graphics.FillRectangle(drawBrush, enemyXValues[i] + 31, enemyYValues[i] + 40, 12, 7);//draws right window

                            drawBrush.Color = Color.Red;
                            e.Graphics.FillPolygon(drawBrush, triangleDetailPoints);//draws ship's central detail

                            break;
                    }

                }
                drawBrush.Color = Color.Red;
                for (int i = 0; i < enemyBulletXValues.Count(); i++)
                {
                    e.Graphics.FillRectangle(drawBrush, enemyBulletXValues[i], enemyBulletYValues[i], 3, 10);//draws enemy light shots
                }
                for (int i = 0; i < enemyDynamicBulletXValues.Count(); i++)
                {
                    e.Graphics.FillRectangle(drawBrush, enemyDynamicBulletXValues[i], enemyDynamicBulletYValues[i], 3, 10);//draws enemy dynamic shots
                }
                for (int i = 0; i < enemyHeavyBulletXValues.Count(); i++)
                {
                    e.Graphics.FillEllipse(drawBrush, enemyHeavyBulletXValues[i], enemyHeavyBulletYValues[i], 10, 10);//draws enemy heavy shots
                }

                //draws pwerups
                for (int i = 0; i < powerupXValues.Count(); i++)
                {
                    drawBrush.Color = Color.Blue;
                    e.Graphics.FillEllipse(drawBrush, powerupXValues[i], powerupYValues[i], 30, 30);
                    drawBrush.Color = Color.White;
                    e.Graphics.DrawString("P", powerupFont, drawBrush, powerupXValues[i] + 7, powerupYValues[i] + 2);
                }

                drawBrush.Color = Color.White;

                //text labels for player stats
                e.Graphics.DrawString("Lives", gameFont, drawBrush, 20, 20);
                e.Graphics.DrawString("Health", gameFont, drawBrush, 20, 50);
                e.Graphics.DrawString("Score:" + score, gameFont, drawBrush, 20, 80);
                if (playerFast) { e.Graphics.DrawString("Double Speed!", gameFont, drawBrush, 20, 110); }
                if (playerInvincible) { e.Graphics.DrawString("Invincible!", gameFont, drawBrush, 20, 140); }
                drawBrush.Color = Color.Red;

                for (int i = 0; i < playerLives; i++)
                {
                    e.Graphics.FillEllipse(drawBrush, 80 + i * 20, 20, 20, 20);//draws lives display
                }
                for (int i = 0; i < playerHealth; i++)
                {
                    e.Graphics.FillRectangle(drawBrush, 85 + i * 15, 50, 10, 20);//draws health display
                }

                //draws shot display
                drawBrush.Color = Color.DarkBlue;
                e.Graphics.FillEllipse(drawBrush, 20, 180, 30, 30);
                drawBrush.Color = Color.Orange;
                switch (fireMode)
                {
                    case "single":
                        e.Graphics.FillRectangle(drawBrush, 30, 185, 10, 20);
                        break;
                    case "double":
                        e.Graphics.FillRectangle(drawBrush, 27, 190, 5, 10);
                        e.Graphics.FillRectangle(drawBrush, 40, 190, 5, 10);
                        break;
                    case "spread":
                        e.Graphics.FillEllipse(drawBrush, 33, 185, 6, 6);
                        e.Graphics.FillEllipse(drawBrush, 26, 192, 6, 6);
                        e.Graphics.FillEllipse(drawBrush, 41, 192, 6, 6);
                        break;
                    case "heavy":
                        e.Graphics.FillEllipse(drawBrush, 25, 185, 20, 20);
                        break;
                }

                //draws pause text
                drawBrush.Color = Color.White;

                if (gamePaused)
                {
                    e.Graphics.DrawString("Game Paused", titleFont, drawBrush, this.Width / 2 - 150, this.Height / 2 - 100);
                    e.Graphics.DrawString("Press P to Unpause or Esc to Exit", gameFont, drawBrush, this.Width / 2 - 165, this.Height / 2);
                }
                drawBrush.Color = Color.Red;
                //draws game oover text
                if (gameOver)
                {
                    e.Graphics.DrawString("Game Over", titleFont, drawBrush, this.Width / 2 - 150, this.Height / 2 - 100);
                    drawBrush.Color = Color.White;
                    e.Graphics.DrawString("Score: " + score, titleFont, drawBrush, this.Width / 2 - 150, this.Height / 2 - 50);
                    e.Graphics.DrawString("Please enter your name", gameFont, drawBrush, this.Width / 2 - 145, this.Height / 2);
                }
            }

            //***************TWO PLAYER PAINT METHOD*****************
            else if (gameState == "two player")
            {
                drawBrush.Color = Color.White;

                for (int i = 0; i < 199; i++)
                {
                    float starY = Convert.ToInt16(starYValues[i]);
                    e.Graphics.FillEllipse(drawBrush, starXValues[i], starY, starSizeValues[i], starSizeValues[i]);
                }

                //draws explosions
                for (int i = 0; i < explosionXValues.Count(); i++)
                {   //explosions become translucent over time.
                    drawBrush.Color = Color.FromArgb(255 - explosionOpacityValues[i], 202 - explosionOpacityValues[i] / 1, 0);
                    e.Graphics.FillEllipse(drawBrush, explosionXValues[i], explosionYValues[i], explosionSizeValues[i], explosionSizeValues[i]);
                    drawBrush.Color = Color.FromArgb(255 - explosionOpacityValues[i], 0, 0);
                    e.Graphics.FillEllipse(drawBrush, explosionXValues[i] + explosionSizeValues[i] / 4, explosionYValues[i] + explosionSizeValues[i] / 4, explosionSizeValues[i] / 2, explosionSizeValues[i] / 2);
                }

                //draws player 1
                if (playerOk)
                {
                    drawBrush.Color = Color.White;

                    Point[] triangle1Points = { new Point(playerX, playerY + 35), new Point(playerX + 5, playerY + 10), new Point(playerX + 10, playerY + 35) };//array for the points of triangle 1
                    Point[] triangle2Points = { new Point(playerX + 20, playerY + 15), new Point(playerX + 25, playerY), new Point(playerX + 30, playerY + 15) };//array for the points of triangle 2
                    Point[] triangle3Points = { new Point(playerX + 40, playerY + 35), new Point(playerX + 45, playerY + 10), new Point(playerX + 50, playerY + 35) };//array for the points of triangle 3


                    e.Graphics.FillRectangle(drawBrush, playerX, playerY + 35, 50, 10);//draws ship base
                    e.Graphics.FillRectangle(drawBrush, playerX + 20, playerY + 15, 10, 20);//draws ship spine

                    drawBrush.Color = Color.DarkRed;

                    e.Graphics.FillPolygon(drawBrush, triangle1Points);//draws left triangle
                    e.Graphics.FillPolygon(drawBrush, triangle2Points);//draws central triangle
                    e.Graphics.FillPolygon(drawBrush, triangle3Points);//draws right triangle

                    e.Graphics.FillRectangle(drawBrush, playerX + 10, playerY + 38, 30, 4);//draws ship base detail
                    e.Graphics.FillRectangle(drawBrush, playerX + 23, playerY + 20, 4, 20);//draws ship spine detail
                }

                //draws player 2
                if (player2Ok)
                {
                    drawBrush.Color = Color.White;

                    Point[] triangle1Points = { new Point(player2X, player2Y + 10), new Point(player2X + 5, player2Y + 35), new Point(player2X + 10, player2Y + 10) };//array for the points of triangle 1
                    Point[] triangle2Points = { new Point(player2X + 20, player2Y + 30), new Point(player2X + 25, player2Y + 45), new Point(player2X + 30, player2Y + 30) };//array for the points of triangle 2
                    Point[] triangle3Points = { new Point(player2X + 40, player2Y + 10), new Point(player2X + 45, player2Y + 35), new Point(player2X + 50, player2Y + 10) };//array for the points of triangle 3


                    e.Graphics.FillRectangle(drawBrush, player2X, player2Y, 50, 10);//draws ship base
                    e.Graphics.FillRectangle(drawBrush, player2X + 20, player2Y + 10, 10, 20);//draws ship spine

                    drawBrush.Color = Color.DarkBlue;

                    e.Graphics.FillPolygon(drawBrush, triangle1Points);//draws left triangle
                    e.Graphics.FillPolygon(drawBrush, triangle2Points);//draws central triangle
                    e.Graphics.FillPolygon(drawBrush, triangle3Points);//draws right triangle

                    e.Graphics.FillRectangle(drawBrush, player2X + 10, player2Y + 3, 30, 4);//draws ship base detail
                    e.Graphics.FillRectangle(drawBrush, player2X + 23, player2Y + 5, 4, 20);//draws ship spine detail
                }

                drawBrush.Color = Color.Orange;

                if (playerFiring)//draws shooting effect
                {
                    if (fireMode == "single" || fireMode == "heavy" || fireMode == "spread")
                    {
                        e.Graphics.FillEllipse(drawBrush, playerX + 22, playerY - 15, 6, 20);
                    }
                    if (fireMode == "double" || fireMode == "spread")
                    {
                        e.Graphics.FillEllipse(drawBrush, playerX + 2, playerY - 5, 6, 20);
                        e.Graphics.FillEllipse(drawBrush, playerX + 42, playerY - 5, 6, 20);
                    }
                }
                if (player2Firing)//draws shooting effect
                {
                    if (p2FireMode == "single" || p2FireMode == "heavy" || p2FireMode == "spread")
                    {
                        e.Graphics.FillEllipse(drawBrush, player2X + 22, player2Y + 45, 6, 20);
                    }
                    if (p2FireMode == "double" || p2FireMode == "spread")
                    {
                        e.Graphics.FillEllipse(drawBrush, player2X + 2, player2Y + 35, 6, 20);
                        e.Graphics.FillEllipse(drawBrush, player2X + 42, player2Y + 35, 6, 20);
                    }
                }

                for (int i = 0; i < bulletXValues.Count(); i++)
                {
                    switch (bulletTypeValues[i])
                    {//switch statement to determine which bullet shape to draw
                        case "light":
                            e.Graphics.FillRectangle(drawBrush, bulletXValues[i], bulletYValues[i], 3, 10);//draws player shots
                            break;
                        case "spread left":
                            e.Graphics.FillEllipse(drawBrush, bulletXValues[i], bulletYValues[i], 3, 6);//draws player shots
                            break;
                        case "spread center":
                            e.Graphics.FillEllipse(drawBrush, bulletXValues[i], bulletYValues[i], 3, 6);//draws player shots
                            break;
                        case "spread right":
                            e.Graphics.FillEllipse(drawBrush, bulletXValues[i], bulletYValues[i], 3, 6);//draws player shots
                            break;
                        case "heavy":
                            e.Graphics.FillEllipse(drawBrush, bulletXValues[i], bulletYValues[i], 10, 10);//draws player shots
                            break;
                    }

                }

                for (int i = 0; i < p2BulletXValues.Count(); i++)
                {
                    switch (p2BulletTypeValues[i])
                    {//switch statement to determine which bullet shape to draw
                        case "light":
                            e.Graphics.FillRectangle(drawBrush, p2BulletXValues[i], p2BulletYValues[i], 3, 10);//draws player shots
                            break;
                        case "spread left":
                            e.Graphics.FillEllipse(drawBrush, p2BulletXValues[i], p2BulletYValues[i], 3, 6);//draws player shots
                            break;
                        case "spread center":
                            e.Graphics.FillEllipse(drawBrush, p2BulletXValues[i], p2BulletYValues[i], 3, 6);//draws player shots
                            break;
                        case "spread right":
                            e.Graphics.FillEllipse(drawBrush, p2BulletXValues[i], p2BulletYValues[i], 3, 6);//draws player shots
                            break;
                        case "heavy":
                            e.Graphics.FillEllipse(drawBrush, p2BulletXValues[i], p2BulletYValues[i], 10, 10);//draws player shots
                            break;
                    }
                }

                for (int i = 0; i < powerupXValues.Count(); i++)
                {
                    drawBrush.Color = Color.Blue;
                    e.Graphics.FillEllipse(drawBrush, powerupXValues[i], powerupYValues[i], 30, 30);
                    drawBrush.Color = Color.White;
                    e.Graphics.DrawString("P", powerupFont, drawBrush, powerupXValues[i] + 7, powerupYValues[i] + 2);
                }

                drawBrush.Color = Color.White;

                //text labels for player stats
                e.Graphics.DrawString("Lives", gameFont, drawBrush, 20, 20);
                e.Graphics.DrawString("Health", gameFont, drawBrush, 20, 50);
                drawBrush.Color = Color.Red;

                for (int i = 0; i < playerLives; i++)
                {
                    e.Graphics.FillEllipse(drawBrush, 80 + i * 20, 20, 20, 20);//draws lives display
                }
                for (int i = 0; i < playerHealth; i++)
                {
                    e.Graphics.FillRectangle(drawBrush, 85 + i * 15, 50, 10, 20);//draws health display
                }

                //draws shot display
                drawBrush.Color = Color.DarkBlue;
                e.Graphics.FillEllipse(drawBrush, 20, 180, 30, 30);
                drawBrush.Color = Color.Orange;
                switch (fireMode)
                {
                    case "single":
                        e.Graphics.FillRectangle(drawBrush, 30, 185, 10, 20);
                        break;
                    case "double":
                        e.Graphics.FillRectangle(drawBrush, 27, 190, 5, 10);
                        e.Graphics.FillRectangle(drawBrush, 40, 190, 5, 10);
                        break;
                    case "spread":
                        e.Graphics.FillEllipse(drawBrush, 33, 185, 6, 6);
                        e.Graphics.FillEllipse(drawBrush, 26, 192, 6, 6);
                        e.Graphics.FillEllipse(drawBrush, 41, 192, 6, 6);
                        break;
                    case "heavy":
                        e.Graphics.FillEllipse(drawBrush, 25, 185, 20, 20);
                        break;
                }

                drawBrush.Color = Color.White;

                //text labels for player stats
                e.Graphics.DrawString("Lives", gameFont, drawBrush, this.Width - 170, 20);
                e.Graphics.DrawString("Health", gameFont, drawBrush, this.Width - 170, 50);
                if (playerFast) { e.Graphics.DrawString("Double Speed!", gameFont, drawBrush, 20, 110); }
                if (playerInvincible) { e.Graphics.DrawString("Invincible!", gameFont, drawBrush, 20, 140); }
                drawBrush.Color = Color.Blue;

                for (int i = 0; i < player2Lives; i++) //player 2 lives display
                {
                    e.Graphics.FillEllipse(drawBrush, (this.Width - 100) + i * 20, 20, 20, 20);//draws lives display
                }
                for (int i = 0; i < player2Health; i++)//player 2 health display
                {
                    e.Graphics.FillRectangle(drawBrush, (this.Width - 100) + i * 15, 50, 10, 20);//draws health display
                }

                //draws p2 shot display
                drawBrush.Color = Color.DarkBlue;
                e.Graphics.FillEllipse(drawBrush, this.Width - 50, 180, 30, 30);
                drawBrush.Color = Color.Orange;
                switch (p2FireMode)
                {
                    case "single":
                        e.Graphics.FillRectangle(drawBrush, this.Width - 40, 185, 10, 20);
                        break;
                    case "double":
                        e.Graphics.FillRectangle(drawBrush, this.Width - 44, 190, 5, 10);
                        e.Graphics.FillRectangle(drawBrush, this.Width - 30, 190, 5, 10);
                        break;
                    case "spread":
                        e.Graphics.FillEllipse(drawBrush, this.Width - 37, 185, 6, 6);
                        e.Graphics.FillEllipse(drawBrush, this.Width - 44, 192, 6, 6);
                        e.Graphics.FillEllipse(drawBrush, this.Width - 30, 192, 6, 6);
                        break;
                    case "heavy":
                        e.Graphics.FillEllipse(drawBrush, this.Width - 45, 185, 20, 20);
                        break;
                }

                //draws pause text
                drawBrush.Color = Color.White;

                if (gamePaused)
                {
                    e.Graphics.DrawString("Game Paused", titleFont, drawBrush, this.Width / 2 - 150, this.Height / 2 - 100);
                    e.Graphics.DrawString("Press P to Unpause or Esc to Exit", gameFont, drawBrush, this.Width / 2 - 165, this.Height / 2);
                }
                if (gameOver)
                {
                    //outputs winning player and congratulatory mesage
                    e.Graphics.DrawString(winner + " Wins!", titleFont, drawBrush, this.Width / 2 - 150, this.Height / 2 - 100);
                }
                drawBrush.Color = Color.Red;
                e.Graphics.DrawString(screenText, titleFont, drawBrush, this.Width / 2, this.Height / 2 - 100);
            }

            else if (gameState == "high scores")
            {
                drawBrush.Color = Color.White;
                e.Graphics.DrawString("High Scores", titleFont, drawBrush, this.Width / 2 - 160, 50);//draws title

                for (int i = 0; i < 10; i++)
                {   //adjusts color based on position in high scores
                    if (i == 0) { drawBrush.Color = Color.Gold; }
                    else if (i == 1) { drawBrush.Color = Color.Silver; }
                    else if (i == 2) { drawBrush.Color = Color.Brown; }
                    else { drawBrush.Color = Color.White; }

                    e.Graphics.DrawString((i + 1).ToString(), powerupFont, drawBrush, 450, 150 + i * 50);//draws place
                    e.Graphics.DrawString(highScoreNames[i], powerupFont, drawBrush, 650, 150 + i * 50);//draws names
                    e.Graphics.DrawString(Convert.ToString(highScores[i]), powerupFont, drawBrush, 850, 150 + i * 50);//draws scores
                }
            }
        }

        //*****************REMOVAL FUNCTIONS**********************
        void removeEnemies(int i)//
        {
            //removes collided enemies
            enemyXValues.Remove(enemyXValues[i]);
            enemyYValues.Remove(enemyYValues[i]);
            enemyTypeValues.Remove(enemyTypeValues[i]);
            enemyHealths.Remove(enemyHealths[i]);
            enemyStartXes.Remove(enemyStartXes[i]);

        }
        //bullet removal
        void removeBullets(int i)
        {
            bulletXValues.Remove(bulletXValues[i]);
            bulletYValues.Remove(bulletYValues[i]);
            bulletTypeValues.Remove(bulletTypeValues[i]);
        }
        //enemy bullet removal
        void removeEnemyBullets(int i)
        {
            enemyBulletXValues.Remove(enemyBulletXValues[i]);//removes offscreen enemy bullets
            enemyBulletYValues.Remove(enemyBulletYValues[i]);
        }
        //enemy dynamic bullet removal
        void removeDynamicBullets(int i)
        {
            enemyDynamicBulletXValues.Remove(enemyDynamicBulletXValues[i]);//removes offscreen enemy bullets
            enemyDynamicBulletYValues.Remove(enemyDynamicBulletYValues[i]);
            enemyDynamicBulletXIncreases.Remove(enemyDynamicBulletXIncreases[i]);
            enemyDynamicBulletYIncreases.Remove(enemyDynamicBulletYIncreases[i]);
        }

        //enemy heavy bullet removal
        void removeHeavyBullets(int i)
        {
            enemyHeavyBulletXValues.Remove(enemyHeavyBulletXValues[i]);
            enemyHeavyBulletYValues.Remove(enemyHeavyBulletYValues[i]);
        }
        //powerup removal
        void removePowerups(int i)
        {
            powerupXValues.Remove(powerupXValues[i]);
            powerupYValues.Remove(powerupYValues[i]);
        }
        //powerup removal
        void removeExplosions(int i)
        {
            explosionXValues.Remove(explosionXValues[i]);
            explosionYValues.Remove(explosionYValues[i]);
            explosionSizeValues.Remove(explosionSizeValues[i]);
            explosionOpacityValues.Remove(explosionOpacityValues[i]);
        }
        //p2 bullet removal
        void removeP2Bullets(int i)
        {
            p2BulletXValues.Remove(p2BulletXValues[i]);
            p2BulletYValues.Remove(p2BulletYValues[i]);
            p2BulletTypeValues.Remove(p2BulletTypeValues[i]);
        }



        //begins one player mode on the one player button click
        private void onePlayerButton_Click(object sender, EventArgs e)
        {
            if (gameState == "title")
            {
                changeTitleVisibility(false);
                gameState = "one player";
                gameTimer.Enabled = true;
                gameTimer.Start();
                this.Focus();
            }
        }

        //back button (from high score screen) click method
        private void returnButton_Click(object sender, EventArgs e)
        {
            gameState = "title";
            gameTimer.Enabled = false;
            Refresh();
            changeTitleVisibility(true);
            returnButton.Visible = false;
        }

        //exit button method
        private void exitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        //two player button click method
        private void twoPlayerButton_Click(object sender, EventArgs e)
        {
            gameState = "two player";
            changeTitleVisibility(false);

            //delays startup by 3 seconds
            screenText = "3";
            Refresh();
            Thread.Sleep(1000);
            screenText = "2";
            Refresh();
            Thread.Sleep(1000);
            screenText = "1";
            Refresh();
            Thread.Sleep(1000);
            screenText = "";

            //starts  game
            gameTimer.Enabled = true;
            gameTimer.Start();
            this.Focus();



        }

        private void enterNameButton_Click(object sender, EventArgs e)
        {
            if (gameOver)
            {
                string name = entryBox.Text;

                //adjusts high scores
                for (int i = 0; i < 10; i++)
                {
                    if (score > highScores[i])
                    {
                        highScores.Insert(i, score);
                        highScoreNames.Insert(i, name);
                        break;//break is necessary to prevent all values from being changed to current score
                    }
                }

                
                Refresh();

                changeTitleVisibility(true);
                entryBox.Visible = false;
                enterNameButton.Visible = false;
                quit();
            }
        }

        //high score button click method
        private void highScoreButton_Click(object sender, EventArgs e)
        {
            gameState = "high scores";
            changeTitleVisibility(false);
            returnButton.Visible = true;
            gameTimer.Enabled = true;
            gameTimer.Start();
            this.Focus();
        }

        //displays a messagebox that show the controls
        private void controlsButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("                          ******CONTROLS******\n\nPlayer 1 uses WASD to move and Space to shoot \nPlayer 2 uses IJKL to move and M to shoot\nPress P to pause game");
        }

        //function to cause the title forms to become invisible or visible
        void changeTitleVisibility(bool state)
        {
            lowerTitleLabel.Visible = state;
            upperTitleLabel.Visible = state;
            nameLabel.Visible = state;
            onePlayerButton.Visible = state;
            twoPlayerButton.Visible = state;
            highScoreButton.Visible = state;
            explosionBox.Visible = state;
            exitButton.Visible = state;
            controlsButton.Visible = state;
        }

        double calculateDistance(int x1, int x2, int y1, int y2)//method to calculate distance for use in collision detection
        {
            return ((Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2))));
        }

        void moveP1()
        {
            if (leftArrowDown == true && playerX > 5)
            {
                playerX -= playerSpeed;
            }
            if (rightArrowDown == true && playerX < this.Width - 70)
            {
                playerX += playerSpeed;
            }
            if (downArrowDown == true && playerY < this.Height - 88)
            {
                playerY += playerSpeed;
            }
            if (upArrowDown == true && playerY > 5)
            {
                playerY -= playerSpeed;
            }
        }

        void moveP2()
        {
            if (jDown == true && player2X > 5)
            {
                player2X -= playerSpeed;
            }
            if (lDown == true && player2X < this.Width - 70)
            {
                player2X += playerSpeed;
            }
            if (kDown == true && player2Y < this.Height - 88)
            {
                player2Y += playerSpeed;
            }
            if (iDown == true && player2Y > 5)
            {
                player2Y -= playerSpeed;
            }
        }

        //runs the code that must be applied to explosions on each tick
        void handleExplosions()
        {
            //******************EXPLOSIONS*************************************************
            for (int i = 0; i < explosionXValues.Count(); i++)
            {   //removes explosions that reach a certain size
                if (explosionSizeValues[i] >= 100)
                {
                    removeExplosions(i);
                }
                else
                {
                    explosionSizeValues[i] += 4;//grows explosion
                    explosionOpacityValues[i] += 8;

                    //recentralizes explosion
                    if (tracker % 2 == 0)
                    {
                        explosionXValues[i] -= 4;
                        explosionYValues[i] -= 4;
                    }
                }
            }
        }


        //clears all game-relevant variables
        void clearVariables()
        {
            playerX = this.Width / 2;
            playerY = this.Height - 100;
            playerHealth = 5;
            playerLives = 3;
            score = 0;
            playerSpeed = 6;
            bulletModulator = 10;
            timeSincePowerup = 0;

            gamePaused = false;
            playerFiring = false;
            playerDying = false;
            gameOver = false;
            playerOk = true;
            playerInvincible = false;
            playerFast = false;

            player2Ok = true;
            player2Lives = 3;
            player2Health = 5;
            player2Firing = false;
            player2X = this.Width / 2;
            player2Y = 100;

            p2FireMode = "single";
            fireMode = "single";
            gameState = "title";

            enemySpawnRate = 1;

            tracker = 0;

            bulletXValues.Clear();
            bulletYValues.Clear();
            bulletTypeValues.Clear();

            enemyXValues.Clear();
            enemyYValues.Clear();
            enemyHealths.Clear();
            enemyStartXes.Clear();
            enemyTypeValues.Clear();

            enemyBulletXValues.Clear();
            enemyBulletYValues.Clear();

            enemyDynamicBulletXValues.Clear();
            enemyDynamicBulletYValues.Clear();
            enemyDynamicBulletXIncreases.Clear();
            enemyDynamicBulletYIncreases.Clear();

            enemyHeavyBulletXValues.Clear();
            enemyHeavyBulletYValues.Clear();

            powerupXValues.Clear();
            powerupYValues.Clear();

            explosionXValues.Clear();
            explosionYValues.Clear();
            explosionSizeValues.Clear();
            explosionOpacityValues.Clear();
        }

        void quit()//returns to title
        {
            gameState = "title";
            gameTimer.Enabled = false;
            clearVariables();
            Refresh();
            changeTitleVisibility(true);

            bgMusicPlayer.Open(new System.Uri(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "bgMusic.mp3")));
            bgMusicPlayer.Play();// loops bg music
        }

        //checks if music is over and restarts it if it is
        void checkMusic()
        {
            if (musicWatch.ElapsedMilliseconds > 26000)
            {   //restarts stopwatch
                musicWatch.Reset();
                musicWatch.Start();

                //plauys bg music
                bgMusicPlayer.Open(new System.Uri(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "bgMusic.mp3")));
                bgMusicPlayer.Play();
            }
        }

        //my splash screen method
        public void SplashScreen()
        {
            Form ss = new Form();
            ss.FormBorderStyle = FormBorderStyle.None;
            ss.Size = new Size(400, 400);
            ss.StartPosition = FormStartPosition.CenterScreen;
            ss.BackColor = Color.Red;
            ss.TransparencyKey = Color.Red;

            DoubleBuffered = true;

            ss.Show();

            Graphics ssGraphics = ss.CreateGraphics();
            SolidBrush ssBrush = new SolidBrush(Color.DarkBlue);
            Font ssFont = new Font("Liberation Mono", 30, FontStyle.Bold);
            Font subtitles = new Font("Liberation Mono", 20);
            ssGraphics.FillEllipse(ssBrush, 0, 0, 400, 400);

            Thread.Sleep(2000);

            for (int i = -50; i < 50; i += 2)
            {
                ssBrush.Color = Color.DarkBlue;
                ssGraphics.FillEllipse(ssBrush, 0, 0, 400, 400);

                ssBrush.Color = Color.Blue;
                ssGraphics.FillEllipse(ssBrush, 180, i, 50, 50);

                ssBrush.Color = Color.LightCyan;
                ssGraphics.DrawString("A", subtitles, ssBrush, 190, i + 10);
                Thread.Sleep(5);

            }

            ssBrush.Color = Color.White;

            for (int i = 90; i > 0; i--)
            {

                ssBrush.Color = Color.DarkBlue;
                ssGraphics.FillEllipse(ssBrush, 0, 0, 400, 400);

                ssBrush.Color = Color.Blue;
                ssGraphics.FillEllipse(ssBrush, 180, 50, 50, 50);

                ssBrush.Color = Color.LightCyan;
                ssGraphics.DrawString("A", subtitles, ssBrush, 190, 60);

                ssBrush.Color = Color.White;
                ssGraphics.TranslateTransform(200, 50);
                ssGraphics.RotateTransform(i);
                ssGraphics.DrawString("Gareth Marks", ssFont, ssBrush, -150, 100);
                ssGraphics.ResetTransform();

                Thread.Sleep(5);
            }

            string[] letters = { "p", "r", "o", "d", "u", "c", "t", "i", "o", "n", "s", };

            ssBrush.Color = Color.Cyan;

            for (int i = 0; i < 10; i++)
            {
                ssGraphics.DrawString(letters[i], subtitles, ssBrush, i * 18 + 70, 200);
                Thread.Sleep(200);
            }


            for (int i = 50; i < 200; i++)
            {
                ssGraphics.FillRectangle(ssBrush, i, 330, 3, 3);
                ssGraphics.FillRectangle(ssBrush, 400 - i, 330, 3, 3);
                Thread.Sleep(10);
            }
            Thread.Sleep(3000);
        }
    }
}
