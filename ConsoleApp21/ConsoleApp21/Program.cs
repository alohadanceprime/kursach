using ConsoleApp15;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace ConsoleApp21
{
    internal class Program
    {
        static Random rand = new Random();
        static List<Squad> best_combination = new List<Squad>();
        static int max_compatibility;
        static List<astronaut> astronauts = new List<astronaut>();
        static List<Squad> squads = new List<Squad>();
        static Stopwatch stopwatch = new Stopwatch();

        static public void print_data()
        {
            for (int i = 0; i < astronauts.Count; i++)
            {
                Console.WriteLine($"Номер астронавта: {astronauts[i].astronaut_number}");
                Console.WriteLine("Совместимость с другими астронавтами");
                foreach (var item in astronauts[i].astronaut_list)
                {
                    Console.WriteLine($"Номер астронавта: {item[0]} Совместимость: {item[1]}");
                }
            }
        }


        static public void get_data_from_txt(string path)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                string[] data_lines = sr.ReadToEnd().Split('\n');
                foreach (var line in data_lines)
                {
                    if (line.Length != 0)
                    {
                        astronaut astr = new astronaut(Convert.ToInt32(line.Split()[0]));
                        for (int i = 1; i < line.Split().Length - 2; i += 2)
                        {
                            astr.astronaut_list.Add(new int[] { Convert.ToInt32(line.Split()[i]), Convert.ToInt32(line.Split()[i + 1]) });
                        }
                        astronauts.Add(astr);
                    }
                }
            }
        }


        static public void write_data_to_txt(string path)
        {
            using (StreamWriter sw = new StreamWriter(path))
            {
                for (int i = 0; i < astronauts.Count; i++)
                {
                    string s = astronauts[i].astronaut_number.ToString();
                    for (int q = 0; q < astronauts[i].astronaut_list.Count; q++)
                    {
                        s += $" {astronauts[i].astronaut_list[q][0]} {astronauts[i].astronaut_list[q][1]}";
                    }
                    sw.WriteLine(s);
                }
            }
        }


        static public void generate_random_data()
        {
            Console.WriteLine("Введите количество астронавтов");
            int kol = Convert.ToInt32(Console.ReadLine());
            for (int i = 1; i < kol+1; i++)
            {
                astronaut astr = new astronaut(i);
                for (int q = 1; q < kol+1; q++)
                {
                    if (i != q)
                    {
                        int[] astr_s = { q, rand.Next(0, 101) };
                        astr.astronaut_list.Add(astr_s);
                    }
                }
                astronauts.Add(astr);
            }
        }


        static public void enter_data()
        {
            Console.WriteLine("Введите количество астронавктов");
            int kol = Convert.ToInt32(Console.ReadLine());
            for (int i = 0; i < kol; i++)
            {
                Console.WriteLine("Введите номер астронавта");
                astronaut astr = new astronaut(Convert.ToInt32(Console.ReadLine()));
                for (int q = 0; q < kol - 1; q++)
                {
                    Console.WriteLine("Введите номер астронавта");
                    int num = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Введите совместимость");
                    int sov = Convert.ToInt32(Console.ReadLine());
                    int[] astr_s = { num, sov };
                    astr.astronaut_list.Add(astr_s);
                }
                astronauts.Add(astr);
            }
        }


        public static void recursion_to_create_squads(List<astronaut> astronauts, int k_chel, int q, List<astronaut> returned_squad)
        {
            if (returned_squad.Count == k_chel)
            {
                squads.Add(new Squad(returned_squad));
            }
            else
            {
                for (int i = q; i < astronauts.Count; i++)
                {
                    List<astronaut> new_squad = new List<astronaut>(returned_squad);
                    new_squad.Add(astronauts[i]);
                    recursion_to_create_squads(astronauts, k_chel, i + 1, new_squad);
                }
            }
        }


        public static void create_squads()
        {
            
            Console.WriteLine("Введите количество человек в отряде");
            int k_chel = Convert.ToInt32(Console.ReadLine());
            stopwatch.Start();
            if (k_chel > astronauts.Count)
            {
                throw new Exception("Not enough astronauts exception");
            }
            recursion_to_create_squads(astronauts, k_chel, 0, new List<astronaut>());
            stopwatch.Stop();
        }


        public static void recursion_to_create_combinations(List<Squad> squads, int k_otr, int q, List<Squad> combination)
        {
            if (combination.Count == k_otr)
            {
                if (choose_best_comb(combination))
                {
                    best_combination = combination;
                }
            }
            else
            {
                for (int i = q; i < squads.Count; i++)
                {
                    List<Squad> new_combination = new List<Squad>(combination);
                    new_combination.Add(squads[i]);
                    if (check_repetition(new_combination))
                    {
                        recursion_to_create_combinations(squads, k_otr, i + 1, new_combination);
                    }
                }
            }
        }


        public static void create_comb()
        {
            Console.WriteLine("Введите количество отрядов которые нужно сформировать");
            int k_otr = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Выберите каким образом найти лучшую совместимость");
            Console.WriteLine("Полный перебор[0]");
            Console.WriteLine("Жадный алгоритм[1]");
            ConsoleKey method_chosen_key = Console.ReadKey().Key;
            Console.WriteLine();
            
            if (k_otr > squads.Count)
            {
                throw new Exception("Not enough astronauts exception");
            }
            switch (method_chosen_key)
            {
                case ConsoleKey.D0:
                    recursion_to_create_combinations(squads, k_otr, 0, new List<Squad>());
                    break;
                case ConsoleKey.D1:
                    greedy_algorithm(squads, k_otr);
                    break;
            }
            
        }   


        public static bool choose_best_comb(List<Squad> combination)
        {
            int current_compatibility = 0;
            for (int i = 0; i < combination.Count; i++)
            {
                current_compatibility += combination[i].squad_compatibility;
            }
            if (current_compatibility >= max_compatibility)
            {
                max_compatibility = current_compatibility;
                return true;
            }
            return false;
        }


        public static bool check_repetition(List<Squad> combination)
        {
            List<int> astronauts = new List<int>();
            for (int i = 0; i < combination.Count; i++)
            {
                for (int q = 0; q < combination[i].squad.Count; q++)
                {
                    if (astronauts.Contains(combination[i].squad[q].astronaut_number))
                    {
                        return false;
                    }
                    else
                    {
                        astronauts.Add(combination[i].squad[q].astronaut_number);
                    }
                }
            }
            return true;
        }

        
        static void print_best_comb()
        {
            Console.WriteLine($"Лучшая совместимость: {max_compatibility}");
            for (int i = 0; i < best_combination.Count; i++)
            {            
                Console.WriteLine($"Отряд номер: {i+1}");
                Console.WriteLine($"Совместимость отряда: {best_combination[i].squad_compatibility}");
                for (int q = 0; q < best_combination[i].squad.Count; q++)
                {
                    Console.WriteLine($"Космонафт номер: {best_combination[i].squad[q].astronaut_number}");
                }
            }
        }
        

        static void greedy_algorithm(List<Squad> squads, int k_otr)
        {
            List<int> astronaut_used = new List<int>(); 
            while (best_combination.Count != k_otr) 
            {
                int max_compatibility1 = 0;
                int best_squad_index = 0;
                for (int i = 0; i < squads.Count; i++)
                {
                    if (squads[i].squad_compatibility >= max_compatibility1)
                    {
                        max_compatibility1 = squads[i].squad_compatibility;
                        best_squad_index = i;
                    }
                }

                max_compatibility += max_compatibility1;
                best_combination.Add(squads[best_squad_index]);
                astronaut_used.Clear();
                for (int i = 0; i < squads[best_squad_index].squad.Count; i++)
                {
                    astronaut_used.Add(squads[best_squad_index].squad[i].astronaut_number);
                }          
                squads.RemoveAt(best_squad_index);
                for (int i = 0; i < squads.Count; i++)
                {
                    for (int q = 0; q < squads[i].squad.Count; q++)
                    {
                        if (astronaut_used.Contains(squads[i].squad[q].astronaut_number))
                        {
                            squads.RemoveAt(i);
                            i--;
                            break;
                        }
                    }
                }
            }
        }


        static void Main(string[] args)
        {
            short action = 3;
            ConsoleKey key;
            string[] menu = {"          Данные не получены", "          Отряды не сформированы",
                "#############################################################",
                    "Сформировать исходные данные", "Вывсети исходные данные",
                "Найти лучшую комбинацию отрядов", "Выход"};
            do
            {
                for (int i = 0; i < menu.Length; i++)
                {
                    if (i == action)
                    {
                        Console.WriteLine(">>>   " + menu[i]);
                        continue;
                    }
                    Console.WriteLine(menu[i]);
                }
                key = Console.ReadKey().Key;
                if (key == ConsoleKey.UpArrow && action > 3)
                {
                    action -= 1;
                }
                else if (key == ConsoleKey.DownArrow && action < menu.Length - 1)
                {
                    action += 1;
                }
                else if (key == ConsoleKey.Enter)
                {
                    if (action == menu.Length-1)
                    {
                        Console.WriteLine("Программа завершена");
                        break;
                    }
                    switch (action)
                    {
                        case 3:
                            Console.Clear();
                            Console.WriteLine("Каким орбразом формировать данные?");
                            Console.WriteLine("Ввести данные с клавиатуры[0]");
                            Console.WriteLine("Сгенерировать случайные данные[1]");
                            Console.WriteLine("Получить данные из txt[2]");
                            ConsoleKey method_chosen_key = Console.ReadKey().Key;
                            Console.WriteLine();
                            switch (method_chosen_key)
                            {
                                case ConsoleKey.D0:
                                    try
                                    {
                                        enter_data();
                                        menu[0] = "данные получены";
                                    }
                                    catch
                                    {
                                        Console.WriteLine("Введенные данные представленны в неверном формате");
                                        Console.Clear();
                                        continue;
                                    }
                                    break;
                                case ConsoleKey.D1:
                                    try
                                    {
                                        generate_random_data();
                                    }
                                    catch
                                    {
                                        Console.WriteLine("Вы ввели не число");
                                        Console.Clear();
                                        continue;
                                    }
                                    break;
                                case ConsoleKey.D2:
                                    try
                                    {
                                        Console.WriteLine("Введите путь к файлу");
                                        get_data_from_txt(Console.ReadLine());
                                    }
                                    catch
                                    {
                                        Console.WriteLine("Неправильный формат пути");
                                        Console.Clear();
                                        continue;
                                    }
                                    break;
                            }
                            Console.WriteLine("Данные получены");
                            Console.ReadKey();
                            break;
                        case 4:
                            Console.Clear();
                            Console.WriteLine("Каким образом выводить данные?");
                            Console.WriteLine("Вывод в консоль[0]");
                            Console.WriteLine("Вывод в txt[1]");
                            ConsoleKey method_chosen_key1 = Console.ReadKey().Key;
                            Console.WriteLine();
                            switch (method_chosen_key1)
                            {
                                case ConsoleKey.D0:
                                    print_data();
                                    Console.ReadKey();
                                    break;
                                case ConsoleKey.D1:
                                    try
                                    {
                                        Console.WriteLine("Введите путь");
                                        write_data_to_txt(Console.ReadLine());
                                    }
                                    catch
                                    {
                                        Console.WriteLine("Введен некоректный путь");
                                        Console.Clear();
                                        continue;
                                    }
                                    break;
                            }
                            break;
                        case 5:
                            try
                            {
                                create_squads();
                                create_comb();
                                print_best_comb();
                                Console.WriteLine($"Время выполнения программы: {stopwatch.Elapsed}");
                                Console.ReadKey();
                            }
                            catch
                            {
                                Console.WriteLine("Для формирования отрядов недостаточно космонавтов");
                                Console.ReadLine();
                                Console.Clear();
                                continue;
                            }
                            break;
                    }
                }
                Console.Clear();
            }
            while (true);          
        }
    }
}
