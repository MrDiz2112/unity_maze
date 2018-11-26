using UnityEngine;
using System.Collections;

public class Instantiation : MonoBehaviour
{
    [Header("Размер лабиринта")]
    [Range(1, 100)]
    public int sizeX; //Размер лабиринта
    [Range(1, 100)]
    public int sizeY;
    [Range(1, 50)]
    public int height;

    [Header("Префабы для лабиринта")]
    public Transform floor; //Префабы для лабиринта
    public Transform wall, stairway, ceiling, enterCell, exitCell;

    private int[] MazeCells = new int[101]; //Массив для генерации

    private int enterNum, exitNum, FirstEnterNum, LastExitNum; //Координаты входа и выхода

    [Header("Контроллер игрока")]
    public GameObject playerPrefab;



    void Start()
    {
        for (int z = 0; z < height * 4; z += 4)
        {
            for (int y = 0; y < sizeY; y++)  //Создание пола
            {
                for (int x = 0; x < sizeX; x++)
                {
                    floor.name = "Floor(" + x + "," + y + ")";
                    Instantiate(floor, new Vector3(x + 0.5f, z, y + 0.5f), Quaternion.identity);

                }
                WallCreating(y, z);
            }
            //===============================================================================Создание потолка

            ceiling.localScale = new Vector3(sizeX, sizeY, 0.1f);
            Instantiate(ceiling, new Vector3(sizeX / 2f, z + 2f, sizeY / 2f), Quaternion.Euler(270f, 0f, 0f));
        }

        EnterExitCreation();

        PlayerCreation();

    }

    private void WallCreating(int NowY, int NowZ)               //Процедура создания стен
    {
        int decision; // Переменная выбора

        int regionNumbers = 1;
        int howManyWallsDown; //Переменная для определения количества создаваемых стен

        NowZ += 1;

        //===========================================================================================
        //===========================================================================================
        if (NowY == 0)  //==================================================Создание начальных границ
        {

            if ((NowZ/4) % 2 != 0)
            {
                enterNum = Random.Range(0, sizeX);
            }
            else
                if (NowZ == 1)
            {
                enterNum = Random.Range(0, sizeX);
            }
            for (int i = 0; i < sizeX; i++)
            {
                MazeCells[i] = i;
                if (i == enterNum)
                {
                    continue;
                }
                Instantiate(wall, new Vector3(i + 0.5f, NowZ, NowY), Quaternion.identity);
            }

            if (NowZ == 1)
                FirstEnterNum = enterNum;

            Instantiate(wall, new Vector3(0, NowZ, NowY + 0.5f), Quaternion.Euler(0f, 90f, 0f));
            Instantiate(wall, new Vector3(sizeX, NowZ, NowY + 0.5f), Quaternion.Euler(0f, 90f, 0f));

            //========================================================================================
            for (int i = 0; i < sizeX; i++) // ==========================Создание границ в 1м ряду====
            {
                decision = Random.Range(1, 3);  //Решаем, ставить стену или нет


                if (decision == 1)
                {
                    Instantiate(wall, new Vector3(i + 1, NowZ, NowY + 0.5f), Quaternion.Euler(0f, 90f, 0f));
                }
                else
                {
                    MazeCells[i + 1] = MazeCells[i]; //Массив для ряда
                }

            }

            MazeCells[sizeX] = 0;

            //=========================================================================================
            for (int i = 0; i < sizeX; i++) //==================Создание нижних границ в первом ряду===
            {
                if (MazeCells[i + 1] == MazeCells[i])
                {
                    regionNumbers += 1;
                }
                else
                {
                    howManyWallsDown = Random.Range(1, regionNumbers);

                    for (int j = (i); j > (i - regionNumbers + howManyWallsDown); j--)
                            {
                                Instantiate(wall, new Vector3(j + 0.5f, NowZ, NowY + 1f), Quaternion.Euler(0f, 0f, 0f));
                                MazeCells[j] = -1;
                            }
                            
                   
                    regionNumbers = 1;

                }

            }

        } //===========================================================================================
        //=============================================================================================
        //============================================Создание N-ой строки=============================

        if ((NowY > 0) && (NowY < sizeY - 1))
        {

            for (int i = 1; i < sizeX; i++) //========Создание новой строки массива
            {
                if (MazeCells[i] == -1)
                {
                    MazeCells[i] = MazeCells[i - 1] + 1;
                    if (MazeCells[i+1]<=MazeCells[i])
                    {
                        int j = i + 1;
                        while (MazeCells[j] == MazeCells[i])
                        {
                            MazeCells[j] = MazeCells[i]++;
                            j++;
                        }
                    }
                }
            }

            //==========================================================================================
            //===========================Создание крайних границ в N-ой строке
            Instantiate(wall, new Vector3(0, NowZ, NowY + 0.5f), Quaternion.Euler(0f, 90f, 0f));
            Instantiate(wall, new Vector3(sizeX, NowZ, NowY + 0.5f), Quaternion.Euler(0f, 90f, 0f));

            //==========================================================================================
            for (int i = 0; i < sizeX; i++) // ==========================Создание границ в N-ом ряду====
            {
                decision = Random.Range(1, 3);  //Решаем, ставить стену или нет


                if (decision == 1)
                {
                    Instantiate(wall, new Vector3(i + 1, NowZ, NowY + 0.5f), Quaternion.Euler(0f, 90f, 0f));
                }
                else
                {
                    if (MazeCells[i] == MazeCells[i + 1]) //Если соседние элементы одинаковы
                    {
                        Instantiate(wall, new Vector3(i + 1, NowZ, NowY + 0.5f), Quaternion.Euler(0f, 90f, 0f));
                    }
                    else
                        MazeCells[i + 1] = MazeCells[i]; //Массив для ряда
                }

            }

            MazeCells[sizeX] = 0;

            //=======================================================================================
            for (int i = 0; i < sizeX; i++) //==================Создание нижних границ в N-ом ряду===
            {
                if (MazeCells[i + 1] == MazeCells[i])
                {
                    regionNumbers += 1;
                }
                else
                {
                    howManyWallsDown = Random.Range(1, regionNumbers);


                            for (int j = (i); j > (i - regionNumbers + howManyWallsDown); j--)
                            {
                                Instantiate(wall, new Vector3(j + 0.5f, NowZ, NowY + 1f), Quaternion.Euler(0f, 0f, 0f));
                                MazeCells[j] = -1;
                            }

                    
                    regionNumbers = 1;

                }

            }
        }

        //=============================================================================================
        //=============================================================================================
        //=====================================================================Создание последнего ряда

        if (NowY == sizeY - 1)
        {
            for (int i = 1; i < sizeX; i++) //========Создание новой строки массива
            {
                if (MazeCells[i] == -1)
                {
                    MazeCells[i] = MazeCells[i - 1] + 1;
                    if (MazeCells[i + 1] <= MazeCells[i])
                    {
                        int j = i + 1;
                        while (MazeCells[j] == MazeCells[i])
                        {
                            MazeCells[j] = MazeCells[i]++;
                            j++;
                        }
                    }
                }
            }

            //==========================================================================================
            //======================Создание крайних границ в последней строке
            Instantiate(wall, new Vector3(0, NowZ, NowY + 0.5f), Quaternion.Euler(0f, 90f, 0f));
            Instantiate(wall, new Vector3(sizeX, NowZ, NowY + 0.5f), Quaternion.Euler(0f, 90f, 0f));

            if ((NowZ / 4) % 2 == 0)
            {
                exitNum = Random.Range(0, sizeX);
            }
            
                LastExitNum = exitNum;

                //==========================================================================================
                for (int i = 0; i < sizeX; i++) // =====================Создание границ в последнем ряду====
            {

                if (MazeCells[i] == MazeCells[i + 1]) //Если соседние элементы одинаковы
                {
                    Instantiate(wall, new Vector3(i + 1, NowZ, NowY + 0.5f), Quaternion.Euler(0f, 90f, 0f));
                }


                //==============================================Создание нижних границ в последнем ряду

                if (i == exitNum)
                {
                    continue;
                }
                Instantiate(wall, new Vector3(i + 0.5f, NowZ, NowY + 1f), Quaternion.Euler(0f, 0f, 0f));

            }
            if ((NowZ / 4 + 1) != height)
            {
                if ((NowZ / 4) % 2 == 0)
                    Instantiate(stairway, new Vector3(exitNum, NowZ - 1, sizeY), Quaternion.Euler(0f, 0f, 0f));
                if ((NowZ / 4) % 2 != 0)
                    Instantiate(stairway, new Vector3(enterNum + 1, NowZ - 1, 0), Quaternion.Euler(0f, 180f, 0f));
            }

        }



    }

    private void PlayerCreation()
    {
        Instantiate(playerPrefab, new Vector3(FirstEnterNum + 0.5f, 0.5f, -0.5f), Quaternion.Euler(0f, 180f, 0f));
    }

    private void EnterExitCreation()
    {
        Instantiate(enterCell, new Vector3(FirstEnterNum , 0, -1f), Quaternion.identity);

        if ((height-1) % 2 == 0)
        {
            Instantiate(exitCell, new Vector3(LastExitNum + 1f, (height - 1) * 4f, sizeY + 1f), Quaternion.Euler(0f, 180f, 0f));
        }
        else
        {
            Instantiate(exitCell, new Vector3(enterNum , (height-1) * 4f, -1f), Quaternion.identity);
        }

    }

    }

