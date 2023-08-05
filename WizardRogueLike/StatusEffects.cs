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

        public virtual bool Update(GameTime gameTime, GameObject target)
        {
            time -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (time <= 0) return true;
            return false;
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

        public override bool Update(GameTime gameTime, GameObject target)
        {
            target.health -= damage * (float)gameTime.ElapsedGameTime.TotalSeconds;
            return base.Update(gameTime, target);
        }

    }

    class Slow : StatusEffect
    {
        float oldSpeed = 0;
        public Slow(float time, float damage) : base(time, damage)
        {
            this.time = time;
            this.damage = damage;
            this.statusColor = Color.Gray;
        }

        public override bool Instantiate(GameObject enemy)
        {
            
            foreach (object effect in enemy.effects)
            {
                if (effect.GetType() == this.GetType()) return true;
            }
            oldSpeed = enemy.speed;
            enemy.speed = oldSpeed / 2;
            Debug.WriteLine(enemy.speed.ToString() + ":" + oldSpeed.ToString());
            return false;
        }

        public override bool Update(GameTime gameTime, GameObject target)
        {
            time -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (time <= 0) 
            {
                target.speed = oldSpeed;
                return true;
            }
            return false;
        }
    }

    partial class Game1
    {
        
    }
}
