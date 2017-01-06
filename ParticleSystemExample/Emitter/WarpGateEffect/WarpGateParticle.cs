using Microsoft.Xna.Framework;
using System;

namespace ParticleSystemExample
{
    public class WarpGateParticle : ParticleSystem.Particle
    {
        float startDistance;
        float minDistance = 0;
        float distance = 0;
        float angle;

        float delay;

        float moveDelay;
        int typ = -1;
        public WarpGateParticle(float maxLifeTime, Vector3 startPosition, float speed, object[] data)
            : base(maxLifeTime, startPosition, speed)
        {
            float scale = (float)data[0];
            float startDistance = (float)data[1];

            this.startDistance = startDistance;
            angle = WarpGateEmitter.Random.Next(0, 360);
            distance = startDistance;

            alpha = 0.0f;

            int perc = WarpGateEmitter.Random.Next(0, 100);
            if (perc <= 5) // Init Particles left/right side
            {
                delay = WarpGateEmitter.Random.Next(0, 500);
                angle = WarpGateEmitter.Random.Next(0, 2) == 0 ? 0f : 180f;
                minDistance = distance - (1.9f * scale);
                this.Scale = 6.67f * scale;
                typ = 1;
            }
            else if (perc <= 27) // Init Particles Ring
            {
                angle = WarpGateEmitter.Random.Next(0, 2) == 0 ? 0f : 180f;
                float angleMultiplyer = WarpGateEmitter.Random.Next(0, 181);
                angle += angleMultiplyer;
                delay = 500 + (11 * angleMultiplyer);
                moveDelay = delay + WarpGateEmitter.Random.Next(50, 250);
                Scale = 8.33f * scale;
                minDistance = distance - (5 * scale);
                typ = 2;
            }
            else if (perc <= 42)
            {
                delay = WarpGateEmitter.Random.Next(3000, 4000);
                angle = WarpGateEmitter.Random.Next(0, 360);
                typ = 4;
                Scale = 3.367f * scale;
                minDistance = WarpGateEmitter.Random.Next(0, Convert.ToInt32(3 * scale));
            }
            else if (perc <= 47) // 1. Loading Wave Particles (Core) 
            {
                delay = WarpGateEmitter.Random.Next(4000, 4500);
                angle = WarpGateEmitter.Random.Next(0, 360);
                typ = 3;
                Scale = 16.67f * scale;
                minDistance = WarpGateEmitter.Random.Next(0, Convert.ToInt32(3 * scale));
            }
            else if (perc <= 55) // 2. Loading Wave Particles (Core)
            {
                delay = WarpGateEmitter.Random.Next(5500, 6000);
                angle = WarpGateEmitter.Random.Next(0, 360);
                typ = 3;
                Scale = 16.67f * scale;
                minDistance = WarpGateEmitter.Random.Next(Convert.ToInt32(2 * scale), Convert.ToInt32(9 * scale));
            }
            else if (perc <= 63) // 3. Loading Wave Particles (Core)
            {
                delay = WarpGateEmitter.Random.Next(7000, 7500);
                angle = WarpGateEmitter.Random.Next(0, 360);
                typ = 3;
                Scale = 16.67f * scale;
                minDistance = WarpGateEmitter.Random.Next(Convert.ToInt32(9 * scale), Convert.ToInt32(15 * scale));
            }
            else if (perc <= 71) // 4. Loading Wave Particles (Core)
            {
                delay = WarpGateEmitter.Random.Next(8500, 9000);
                angle = WarpGateEmitter.Random.Next(0, 360);
                typ = 3;
                Scale = 16.67f * scale;
                minDistance = WarpGateEmitter.Random.Next(Convert.ToInt32(15 * scale), Convert.ToInt32(25 * scale));
            }
            else // Wave Particles 
            {
                Scale = 16.67f * scale;
                delay = WarpGateEmitter.Random.Next(10000, 10500);
                angle = WarpGateEmitter.Random.Next(0, 360);
                typ = 4;
            }

            currentPosition = new Vector3((float)(currentPosition.X + startDistance * Math.Cos((Math.PI / 180) * angle)),
                                (float)(currentPosition.Y + startDistance * Math.Sin((Math.PI / 180) * angle)),
                                0);
        }


        private void MoveInitlizedRing(float elapsedTime)
        {
            angle += (0.1f * elapsedTime) * (WarpGateEmitter.Random.Next(0, 2) == 0 ? -1 : 1);
            distance += (speed * elapsedTime) * (WarpGateEmitter.Random.Next(0, 2) == 0 ? -1 : 1);
            FixDistance();
        }

        private void MoveCreateRing(float elapsedTime)
        {
            MoveLoadingWave(elapsedTime, false);
        }

        private void MoveLoadingWave(float elapsedTime, bool once)
        {
            if (!once || distance > minDistance)
            {
                distance -= (speed * elapsedTime);
                FixDistance();

                if (distance == minDistance && !once)
                    distance = startDistance;
            }
            else
            {
                MoveCore(elapsedTime);
            }
        }

        private void MoveCore(float elapsedTime)
        {
            angle += WarpGateEmitter.Random.Next(-3, 4) * elapsedTime;
            distance += (speed * elapsedTime) * (WarpGateEmitter.Random.Next(0, 2) == 0 ? -1 : 1);
            FixDistance();
        }

        private void MoveClose()
        {

        }

        private void MoveOutOfEnergy()
        {

        }

        private void FixDistance()
        {
            if (distance < minDistance)
                distance = minDistance;
            else if (distance > startDistance)
                distance = startDistance;
        }

        float elapsedTime;
        public override void Update(GameTime gameTime, Vector3 camPos, bool faceToCam = false)
        {
            if (this.isActive)
            {
                float elapsedMs = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                elapsedTime += elapsedMs;
                if (elapsedTime > delay)
                {
                    alpha = 0.2f;

                    //Move
                    switch (typ)
                    {
                        case 1:
                            MoveInitlizedRing(elapsedMs);
                            break;
                        case 2:
                            if (elapsedTime > moveDelay)
                                MoveCreateRing(elapsedMs);
                            break;
                        case 3:
                            MoveLoadingWave(elapsedMs, true);
                            break;
                        case 4:
                            MoveLoadingWave(elapsedMs, false);
                            break;
                    }

                    currentPosition = new Vector3((float)(StartPosition.X + distance * Math.Cos((Math.PI / 180) * angle)),
                                (float)(StartPosition.Y + distance * Math.Sin((Math.PI / 180) * angle)),
                                0);
                }
            }
            base.Update(gameTime, camPos, faceToCam);
        }

        public override void CustomEvent(string eventName, params object[] args)
        {
            if (eventName == "Shutdown")
            {

            }
            else if (eventName == "Collapse")
            {
            }
        }
    }
}
