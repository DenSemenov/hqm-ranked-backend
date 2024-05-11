using ReplayHandler.Classes;

namespace hqm_ranked_backend.Models.ViewModels
{
    public class ReplayViewerViewModel
    {
        public int Index { get; set; }
        public List<ReplayViewerFragmentViewModel> Fragments { get; set; }
        public ReplayTick[] Data { get; set; }
    }

    public class ReplayViewerFragmentViewModel
    {
        public int Index { get; set; }
        public uint Min { get; set; }
        public uint Max { get; set; }
    }
}
