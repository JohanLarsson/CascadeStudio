namespace CascadeStudio
{
    public enum HaarTypes
    {
        /// <summary>
        /// BASIC use only upright features
        /// http://docs.opencv.org/master/dc/d88/tutorial_traincascade.html
        /// </summary>
        Basic,

        /// <summary>
        /// ??
        /// http://docs.opencv.org/master/dc/d88/tutorial_traincascade.html
        /// </summary>
        Core,

        /// <summary>
        /// ALL uses the full set of upright and 45 degree rotated feature set. See [100] for more details.
        /// http://docs.opencv.org/master/dc/d88/tutorial_traincascade.html
        /// </summary>
        All,
    }
}