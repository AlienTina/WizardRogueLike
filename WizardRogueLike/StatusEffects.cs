using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardRogueLike
{
    public abstract class StatusEffect
    {
        public float time { get; set; }
        public float damage { get; set; }
        public Color statusColor { get; set; }

        public bool hasReacted = false;

        public StatusEffect(float time, float damage)
        {
            this.time = time;
            this.damage = damage;
            this.statusColor = Color.White;
        }

        public virtual bool Instantiate(GameObject enemy)
        {
            foreach (object effect in enemy.effects)
            {
                if (effect.GetType() == this.GetType()) return true;
            }
            return false;
        }

        public virtual bool Update(Game1 game, GameTime gameTime, GameObject target)
        {
            time -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (time <= 0) return true;
            return false;
        }

        public virtual void Hit(Game1 game, GameObject target)
        {

        }
    }
    

    

    class Stun : StatusEffect
    {
        float oldSpeed = 0;
        public Stun(float time, float damage) : base(time, damage)
        {
            this.time = time;
            this.damage = damage;
            this.statusColor = Color.Purple;
        }

        public override bool Instantiate(GameObject enemy)
        {

            foreach (object effect in enemy.effects)
            {
                if (effect.GetType() == this.GetType()) return true;
            }
            
            //enemy.health -= 2 + damage;
            return false;
        }

        public override bool Update(Game1 game, GameTime gameTime, GameObject target)
        {
            target.speed = 0;
            time -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (time <= 0)
            {
                target.speed = target.baseSpeed;
                return true;
            }
            return false;
        }
    }

    class Frostbite : StatusEffect
    {
        float timeElapsed = 1;
        public Frostbite(float time, float damage) : base(time, damage)
        {
            this.time = time;
            this.damage = damage;
            this.statusColor = Color.Cyan;
        }

        public override bool Instantiate(GameObject enemy)
        {

            foreach (object effect in enemy.effects)
            {
                if (effect.GetType() == this.GetType()) return true;
            }

            return false;
        }

        public override bool Update(Game1 game, GameTime gameTime, GameObject target)
        {
            timeElapsed -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            target.speed = 0;
            time -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (timeElapsed <= 0)
            {
                target.speed = target.baseSpeed / 2;
            }
            if (time <= 0)
            {
                target.speed = target.baseSpeed;
                return true;
            }
            return false;
        }
    }

    class DarkCorrosion : StatusEffect
    {
        public DarkCorrosion(float time, float damage) : base(time, damage)
        {
            this.time = time;
            this.damage = damage;
            this.statusColor = Color.LimeGreen;
        }

        public override bool Update(Game1 game, GameTime gameTime, GameObject target)
        {
            target.health -= damage * (float)gameTime.ElapsedGameTime.TotalSeconds;
            return base.Update(game, gameTime, target);
        }

    }

    class Slow : StatusEffect
    {
        public Slow(float time, float damage) : base(time, damage)
        {
            this.time = time;
            this.damage = damage;
            this.statusColor = Color.White;
        }

        public override bool Instantiate(GameObject enemy)
        {
            foreach (object effect in enemy.effects)
            {
                if (effect.GetType() == this.GetType()) return true;
            }
            enemy.speed = enemy.baseSpeed / 2;
            return false;
        }

        public override bool Update(Game1 game, GameTime gameTime, GameObject target)
        {
            time -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (time <= 0)
            {
                target.speed = target.baseSpeed;
                return true;
            }
            return false;
        }
    }
    class Petrified : StatusEffect
    {
        bool toRemove = false;
        public Petrified(float time, float damage) : base(time, damage)
        {
            this.time = time;
            this.damage = damage;
            this.statusColor = Color.DarkGray;
        }

        public override bool Instantiate(GameObject enemy)
        {
            foreach (object effect in enemy.effects)
            {
                if (effect.GetType() == this.GetType()) return true;
            }
            enemy.speed = 0;
            //enemy.defenseShred++;
            return false;
        }

        public override bool Update(Game1 game, GameTime gameTime, GameObject target)
        {
            time -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (time <= 0 || toRemove)
            {
                //target.defenseShred--;
                target.speed = target.baseSpeed;
                return true;
            }
            return false;
        }

        public override void Hit(Game1 game, GameObject target)
        {
            if (toRemove) return;
            target.health -= 5;
            toRemove = true;
            base.Hit(game, target);
        }
    }
    class Frozen : StatusEffect
    {
        bool toRemove = false;
        public Frozen(float time, float damage) : base(time, damage)
        {
            this.time = time;
            this.damage = damage;
            this.statusColor = Color.LightBlue;
        }

        public override bool Instantiate(GameObject enemy)
        {
            foreach (object effect in enemy.effects)
            {
                if (effect.GetType() == this.GetType()) return true;
            }
            enemy.speed = 0;
            //enemy.defenseShred++;
            return false;
        }

        public override bool Update(Game1 game, GameTime gameTime, GameObject target)
        {
            time -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (time <= 0 || toRemove)
            {
                //target.defenseShred--;
                target.speed = target.baseSpeed;
                return true;
            }
            return false;
        }

        public override void Hit(Game1 game, GameObject target)
        {
            if (toRemove) return;
            target.health -= 5;
            toRemove = true;
            base.Hit(game, target);
        }
    }
    class Confused : StatusEffect
    {
        public Confused(float time, float damage) : base(time, damage)
        {
            this.time = time;
            this.damage = damage;
            this.statusColor = Color.White;
            this.time = 1;
        }

        public override bool Instantiate(GameObject enemy)
        {
            foreach (object effect in enemy.effects)
            {
                if (effect.GetType() == this.GetType()) return true;
            }
            enemy.isTargetPlayer = false;
            return false;
        }

        public override bool Update(Game1 game, GameTime gameTime, GameObject target)
        {
            Vector2 newTarget = game.generateRandomPosition();
            target.target = newTarget;
            time -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (time <= 0)
            {
                target.isTargetPlayer = true;
                return true;
            }
            return false;
        }
    }

    class Weakened : StatusEffect
    {
        public Weakened(float time, float damage) : base(time, damage)
        {
            this.time = time;
            this.damage = damage;
            this.statusColor = Color.Gray;
            this.time = 2;
        }

        public override bool Instantiate(GameObject enemy)
        {
            foreach (object effect in enemy.effects)
            {
                if (effect.GetType() == this.GetType()) return true;
            }
            enemy.defenseShred++;
            return false;
        }

        public override bool Update(Game1 game, GameTime gameTime, GameObject target)
        {
            time -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (time <= 0)
            {
                target.defenseShred--;
                return true;
            }
            return false;
        }
    }


    #region elements

    class Fire : StatusEffect
    {
        public Fire(float time, float damage) : base (time, damage)
        {
            this.time = time;
            this.damage = damage;
            this.statusColor = Color.Red;
        }
    }
    class Poisoned : StatusEffect
    {
        public Poisoned(float time, float damage) : base(time, damage)
        {
            this.time = time;
            this.damage = damage;
            this.statusColor = Color.LimeGreen;
        }

        public override bool Update(Game1 game, GameTime gameTime, GameObject target)
        {
            target.health -= damage * (float)gameTime.ElapsedGameTime.TotalSeconds;
            return base.Update(game, gameTime, target);
        }

    }

    class Ice : StatusEffect
    {
        public Ice(float time, float damage) : base(time, damage)
        {
            this.time = time;
            this.damage = damage;
            this.statusColor = Color.LightBlue;
        }

        public override bool Instantiate(GameObject enemy)
        {

            foreach (object effect in enemy.effects)
            {
                if (effect.GetType() == this.GetType()) return true;
            }
            return false;
        }

        public override bool Update(Game1 game, GameTime gameTime, GameObject target)
        {
            time -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (time <= 0)
            {
                return true;
            }
            return false;
        }
    }

    class Lightning : StatusEffect
    {
        public Lightning(float time, float damage) : base(time, damage)
        {
            this.time = time;
            this.damage = damage;
            this.statusColor = Color.Yellow;
        }
    }

    class Water : StatusEffect
    {
        public Water(float time, float damage) : base(time, damage)
        {
            this.time = time;
            this.damage = damage;
            this.statusColor = Color.Blue;
        }
    }

    class Wind : StatusEffect
    {
        public Wind(float time, float damage) : base(time, damage)
        {
            this.time = time;
            this.damage = damage;
            this.statusColor = Color.LightCyan;
        }
    }

    class Nature : StatusEffect
    {
        float timeElapsed = 0.5f;
        public Nature(float time, float damage) : base(time, damage)
        {
            this.time = time;
            this.damage = damage;
            this.statusColor = Color.Green;
        }
        public override bool Update(Game1 game, GameTime gameTime, GameObject target)
        {
            timeElapsed -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            target.speed = 0;
            time -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (timeElapsed <= 0)
            {
                target.speed = target.baseSpeed;
            }
            if(time <= 0)
            {
                target.speed = target.baseSpeed;
                return true;
            }
            return false;
        }
    }

    class Ground : StatusEffect
    {
        public Ground(float time, float damage) : base(time, damage)
        {
            this.time = time;
            this.damage = damage;
            this.statusColor = Color.Brown;
        }
    }

    class Dark : StatusEffect
    {
        public Dark(float time, float damage) : base(time, damage)
        {
            this.time = time;
            this.damage = damage;
            this.statusColor = Color.DarkGray;
        }
    }

    #endregion


    partial class Game1
    {
        
    }
}
