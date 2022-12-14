using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamanimation;
using Xamarin.Forms;
using UXDivers.Grial;

namespace Eleos3
{
    [ContentProperty(nameof(Animations))]
    public class AnimationWrapper : ITriggerAction
    {
        public AnimationWrapper()
        {
            Animations = new Collection<AnimationBase>();
        }

        public Collection<AnimationBase> Animations { get; set; }

        public Task Execute()
        {
            return Task.WhenAll(Animations.Select(x => x.Begin()));
        }
    }
}
