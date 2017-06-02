namespace CascadeStudio
{
    /// <summary>
    /// http://docs.opencv.org/master/dc/d88/tutorial_traincascade.html
    /// </summary>
    public enum ClassifierType
    {
        /// <summary>
        /// Discrete AdaBoost
        /// </summary>
        DAB,

        /// <summary>
        /// Real AdaBoost
        /// </summary>
        RAB,

        /// <summary>
        /// LogitBoost
        /// </summary>
        LB,

        /// <summary>
        /// Gentle AdaBoost.
        /// </summary>
        GAB,
    }
}