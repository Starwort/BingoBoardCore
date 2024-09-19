using Terraria;

namespace BingoBoardCore.Trackers {
    public abstract class PlayerAttackTracker : PlayerTracker {
        public sealed override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            processHitNPC(target, hit);
        }

        public sealed override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone) {
            processHitNPC(target, hit);
        }

        protected virtual void onHit(NPC target, NPC.HitInfo hit) {
        }

        protected virtual void onKill(NPC target, NPC.HitInfo hit) {
        }

        void processHitNPC(NPC target, NPC.HitInfo hit) {
            onHit(target, hit);
            if (target.life <= 0) {
                onKill(target, hit);
            }
        }
    }
}
