using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RPG{
    internal class Program{
        enum Classes{
            Warrior,
            Mage,
            Archer
        }

        static async Task Main(string[] args){
            string playerClass = "";
            Console.WriteLine("Enter your name:");
            Player player = new Player(Console.ReadLine());
            Console.WriteLine("Choose your class: 0 for Warrior, 1 for Mage, 2 for Archer");
            string classChoice = Console.ReadLine();
            switch (classChoice){
                case "0":
                    playerClass = Classes.Warrior.ToString();
                    player.AssignClass(Classes.Warrior);
                    break;
                case "1":
                    playerClass = Classes.Mage.ToString();
                    player.AssignClass(Classes.Mage);
                    break;
                case "2":
                    playerClass = Classes.Archer.ToString();
                    player.AssignClass(Classes.Archer);
                    break;
                default:
                    Console.WriteLine("Invalid class choice. Defaulting to Warrior.");
                    playerClass = Classes.Warrior.ToString();
                    player.AssignClass(Classes.Warrior);
                    break;
            }
            Console.Clear();
            Console.WriteLine($"Welcome, {player.Name}! Class: {player.Class}, HP: {player.Hp}/{player.MaxHp}, MP: {player.Mp}/{player.MaxMp}");
            Console.WriteLine();
            var enemies = new List<Enemy>{
                Enemy.Create(EnemyType.Goblin),
                Enemy.Create(EnemyType.Orc),
                Enemy.Create(EnemyType.Wyrm)
            };

            int restsRemaining = 3;
            for (int i = 0; i < enemies.Count; i++){
                var enemy = enemies[i];
                Console.WriteLine($"Encounter {i + 1}: A {enemy.Name} appeared! HP: {enemy.Hp}");
                bool beforeFight = true;

                while (beforeFight){
                    Console.WriteLine("Choose: 1 - Fight, 2 - Rest, 3 - View status");
                    var opt = Console.ReadLine();
                    if (opt == "1") { beforeFight = false; }
                    else if (opt == "2"){
                        if (restsRemaining > 0){
                            player.Rest();
                            restsRemaining--;
                            Console.WriteLine($"You rested. HP: {player.Hp}/{player.MaxHp}, MP: {player.Mp}/{player.MaxMp}. Rests remaining: {restsRemaining}");
                        }
                        else{
                            Console.WriteLine("No rests remaining.");
                        }
                    }
                    else if (opt == "3"){
                        Console.WriteLine(player.Status());
                    }
                    else{
                        Console.WriteLine("Invalid option.");
                    }
                }

                // Combat starts
                while (player.IsAlive && enemy.IsAlive){
                    // Player's turn
                    Console.WriteLine();
                    Console.WriteLine(player.Class == Classes.Warrior ? "Warrior turn:" + Environment.NewLine + "1-Slash(6dmg) " + Environment.NewLine +
                        "2-ShieldBlock(reduce dmg) " + Environment.NewLine +
                        "3-BattleCry(+atk)" :

                                      player.Class == Classes.Mage ? "Mage turn:" + Environment.NewLine + "1-Fireball(10dmg, cost mp 5) " + Environment.NewLine +
                                      "2-IceBarrier(5dmg + reduce, cost mp 3) " + Environment.NewLine +
                                      "3-ArcaneBlast(8dmg, cost mp 4)" :

                                      "Archer turn:" + Environment.NewLine + "1-RapidShot(7dmg) " + Environment.NewLine +
                                      "2-PoisonArrow(3dmg/turn) " + Environment.NewLine +
                                      "3-EagleEye(+crit)");
                    Console.WriteLine("Choose action (1/2/3):");
                    var action = Console.ReadLine();

                    var playerActionResult = player.UseSkill(action, enemy);
                    Console.WriteLine(playerActionResult);

                    if (!enemy.IsAlive)
                    {
                        Console.WriteLine($"{enemy.Name} was defeated!");
                        // small reward
                        player.GainAfterBattle();
                        break;
                    }
                    // Enemy's turn
                    var enemyResult = enemy.Attack(player);
                    Console.WriteLine(enemyResult);

                    if (!player.IsAlive)
                    {
                        Console.WriteLine("You were defeated. Game over.");
                        return;
                    }

                    // Apply effects per turn (e.g., poison)
                    player.TickStatusEffects();
                    enemy.TickStatusEffects();
                }

                Console.WriteLine($"Encounter {i + 1} completed.");
                Console.WriteLine();
            }

            Console.WriteLine("Congratulations! You defeated all the enemies.");
            Console.WriteLine(player.Status());
            await Task.Delay(5000);
            Console.Clear();
        }

        private class Player{
            public string Name { get; }
            public Classes Class { get; private set; }
            public int MaxHp { get; private set; } = 100;
            public int Hp { get; private set; }
            public int MaxMp { get; private set; } = 10;
            public int Mp { get; private set; }
            public int BaseAttack { get; private set; } = 5;
            private bool shieldBlockActive = false;
            private int poisonTicksRemaining = 0;

            public Player(string name){
                Name = string.IsNullOrWhiteSpace(name) ? "Hero" : name;
                Hp = MaxHp;
                Mp = MaxMp;
            }

            public void AssignClass(Classes cls){
                Class = cls;
                switch (cls){
                    case Classes.Warrior:
                        MaxHp += 20;
                        MaxMp += 5;
                        break;
                    case Classes.Mage:
                        MaxHp += 10;
                        MaxMp += 15;
                        break;
                    case Classes.Archer:
                        MaxHp += 15;
                        MaxMp += 10;
                        break;
                }
                Hp = MaxHp;
                Mp = MaxMp;
            }

            public string UseSkill(string choice, Enemy target)
            {
                choice = choice?.Trim();    
                switch (Class){
                    case Classes.Warrior:
                        return UseWarriorSkill(choice, target);
                    case Classes.Mage:
                        return UseMageSkill(choice, target);
                    case Classes.Archer:
                        return UseArcherSkill(choice, target);
                    default:
                        return "Invalid class.";
                }
            }
            private string UseWarriorSkill(string choice, Enemy target)
            {
                switch (choice)
                {
                    case "1":
                        int dmg = 6 + BaseAttack;
                        target.TakeDamage(dmg);
                        return $"You used Slash and dealt {dmg} damage.";
                    case "2":
                        shieldBlockActive = true;
                        return "You used Shield Block: damage reduction activated for the next attack.";
                    case "3":
                        BaseAttack += 1;
                        return "You used Battle Cry: attack temporarily increased.";
                    default:
                        return "Invalid choice.";
                }
            }
            private string UseMageSkill(string choice, Enemy target)
            {
                switch (choice)
                {
                    case "1":
                        if (Mp < 5) return "Insufficient MP.";
                        Mp -= 5;
                        int dmg = 10 + BaseAttack;
                        target.TakeDamage(dmg);
                        return $"You used Fireball and dealt {dmg} damage.";
                    case "2":
                        if (Mp < 3) return "Insufficient MP.";
                        Mp -= 3;
                        target.TakeDamage(5);
                        shieldBlockActive = true;
                        return "You used Ice Barrier: 5 damage + barrier (reduces damage) applied.";
                    case "3":
                        if (Mp < 4) return "Insufficient MP.";
                        Mp -= 4;
                        target.TakeDamage(8);
                        return "You used Arcane Blast: 8 damage.";
                    default:
                        return "Invalid choice.";
                }
            }
            private string UseArcherSkill(string choice, Enemy target)
            {
                switch (choice)
                {
                    case "1":
                        int dmg = 7 + BaseAttack;
                        target.TakeDamage(dmg);
                        return $"You used Rapid Shot and dealt {dmg} damage.";
                    case "2":
                        target.ApplyPoison(2, 2); // 2 damage per 2 turns
                        return "You used Poison Arrow: target poisoned.";
                    case "3":
                        BaseAttack += 1;
                        return "You used Eagle Eye: critical chance temporarily increased.";
                    default:
                        return "Invalid choice.";
                }
            }

            public void TakeDamage(int amount){
                if (shieldBlockActive){
                    amount = (int)Math.Ceiling(amount * 0.75);
                    shieldBlockActive = false;
                }

                Hp -= amount;
                if (Hp < 0) Hp = 0;
            }

            public void ApplyPoison(int damagePerTurn, int ticks){
                poisonTicksRemaining = Math.Max(poisonTicksRemaining, ticks);
            }

            public void TickStatusEffects(){
                if (poisonTicksRemaining > 0){
                    int dmg = 3;
                    Hp -= dmg;
                    poisonTicksRemaining--;
                    Console.WriteLine($"You take {dmg} poison damage. HP remaining: {Hp}/{MaxHp}");
                    if (Hp < 0) Hp = 0;
                }
            }

            public async Task Rest(){
                // regenerate 50% of max HP and fully restore MP
                int hpRestore = Math.Max(1, MaxHp / 2);
                Hp = Math.Min(MaxHp, Hp + hpRestore);
                Mp = MaxMp;
                await Task.Delay(6000); // simulate resting time
                Console.Clear();
            }

            public bool IsAlive => Hp > 0;

            public void GainAfterBattle(){
                // simple leveling up: restore some HP and MP after battle
                Hp = Math.Min(MaxHp, Hp + 10);
                Mp = Math.Min(MaxMp, Mp + 3);
            }

            public string Status(){
                return $"[{Name}] Class: {Class}, HP: {Hp}/{MaxHp}, MP: {Mp}/{MaxMp}, Attack: {BaseAttack}";
            }
        }

        private enum EnemyType { Goblin, Orc, Wyrm }

        private class Enemy
        {
            public string Name { get; private set; }
            public int Hp { get; private set; }
            public int AttackPower { get; private set; }
            private int poisonTicks = 0;
            private int poisonDamagePerTick = 0;

            public bool IsAlive => Hp > 0;

            public static Enemy Create(EnemyType type)
            {
                switch (type)
                {
                    case EnemyType.Goblin:
                        return new Enemy { Name = "Goblin", Hp = 30, AttackPower = 6 };
                    case EnemyType.Orc:
                        return new Enemy { Name = "Orc", Hp = 50, AttackPower = 10 };
                    case EnemyType.Wyrm:
                        return new Enemy { Name = "Wyrm", Hp = 90, AttackPower = 14 };
                    default:
                        return new Enemy { Name = "Monster", Hp = 20, AttackPower = 5 };
                }
            }

            public void TakeDamage(int amount)
            {
                Hp -= amount;
                if (Hp < 0) Hp = 0;
            }

            public string Attack(Player player)
            {
                int dmg = AttackPower;
                player.TakeDamage(dmg);
                return $"{Name} attacks and deals {dmg} damage. Player HP: {player.Hp}/{player.MaxHp}. Player MP: {player.Mp}/{player.MaxMp}";
            }

            public void ApplyPoison(int dmgPerTurn, int ticks)
            {
                poisonDamagePerTick = dmgPerTurn;
                poisonTicks = Math.Max(poisonTicks, ticks);
            }

            public void TickStatusEffects()
            {
                if (poisonTicks > 0)
                {
                    Hp -= poisonDamagePerTick;
                    poisonTicks--;
                    Console.WriteLine($"{Name} takes {poisonDamagePerTick} poison damage. HP: {Hp}");
                    if (Hp < 0) Hp = 0;
                }
            }
        }
    }
}