using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Aints
{
	public class ConstantsHolder
	{
		#region Singleton
		private static ConstantsHolder singleton = null;
		[Browsable(false)] //do not display this in the propertygrid
		public static ConstantsHolder Singleton
		{
			get
			{
				if (singleton == null)
				{
					singleton = ConstantsHolder.OpenXml();
					if (singleton == null) //xml deserialization failed
					{
						singleton = new ConstantsHolder();
					}
				}
				return singleton;
			}
		}

		/// <summary>
		/// You should NOT call this constructor
		/// and call the singleton instead if you want an instance of the class.
		/// The only purpose of this ctor is to allow xml deserialization.
		/// </summary>
		public ConstantsHolder()
		{

		}
		#endregion

		#region props

		#region general properties

		[CategoryAttribute("Ants Properties"),
		  Description("Maximum amount of hit points for an ant")]
		public int LifeMax { get; set; }
		[CategoryAttribute("Ants Properties"),
		  Description("Pheromones dropping period when an ant goes for food (0 means nothing is dropped)")]
		public int PheromonesGoFood { get; set; }
		[CategoryAttribute("Ants Properties"),
		  Description("Pheromones dropping period when the ant do carry food (0 means nothing is dropped)")]
		public int PheromonesFood { get; set; }
		[CategoryAttribute("Ants Properties"),
		  Description("Maximum quantity of food carried by one ant")]
		public int CarryMax { get; set; }
		[CategoryAttribute("Ants Properties"),
		  Description("How close to something the ant should be to notice it (in pixels)")]
		public int Vision { get; set; }
		[CategoryAttribute("Ants Properties"),
		  Description("How close to food the ant should be to pick it up (in pixels)")]
		public int EatingRadius { get; set; }
		[CategoryAttribute("Ants Properties"),
		  Description("An ant dies if it becomes more hungry than this value")]
		public int Starvation { get; set; }


		[CategoryAttribute("World Properties"),
		  Description("Two pheromones are fusionned into one if they are closer than this value.")]
		public float PheroFusionRadius { get; set; }
		[CategoryAttribute("World Properties"),
		  Description("Each pheromone smell is multiplied by this value each frame.")]
		public float PheroEvaporationRate { get; set; }


		[CategoryAttribute("Hill Properties"),
		  Description("number of frames between two larva being created (there is 60 frames in one second)")]
		public int LarvaSpawnCooldown { get; set; }
		[CategoryAttribute("Hill Properties"),
		  Description("Under this threshold, no new ants will be produced")]
		public float BirthMinFood { get; set; }
		[CategoryAttribute("Hill Properties"),
		  Description("the cost in food of a larva")]
		public float LarvaCost { get; set; }

		#endregion

		#region behavior properties

		[CategoryAttribute("Ants Behaviors"),
		  Description("The previous deplacement vector will be added this times to the new one")]
		public float _GlobalInertia { get; set; }

		#region scout
		[CategoryAttribute("Ants Behaviors"),
		  Description("The highest the value, the more random the ants behave (when looking for food)")]
		public float ScoutRandom { get; set; }
		[CategoryAttribute("Ants Behaviors"),
		  Description("The highest the value, the more focused on the goal the ants behave (when looking for food)")]
		public float ScoutGoal { get; set; }
		[CategoryAttribute("Ants Behaviors"),
		  Description("The highest the value, the more affected by the pheromones the ants will be (when looking for food)")]
		public float ScoutPheromones { get; set; } 
		#endregion

		#region gofood
		[CategoryAttribute("Ants Behaviors"),
		  Description("The highest the value, the more random the ants behave (when going to a known food location)")]
		public float GoFoodRandom { get; set; }
		[CategoryAttribute("Ants Behaviors"),
		  Description("The highest the value, the more focused on the goal the ants behave (when going to a known food location)")]
		public float GoFoodGoal { get; set; }
		[CategoryAttribute("Ants Behaviors"),
		  Description("The highest the value, the more affected by the pheromones the ants will be (when going to a known food location)")]
		public float GoFoodPheromones { get; set; } 
		#endregion

		#region bringfood
		[CategoryAttribute("Ants Behaviors"),
		  Description("The highest the value, the more random the ants behave (when taking food back to the hill)")]
		public float BringFoodRandom { get; set; }
		[CategoryAttribute("Ants Behaviors"),
		  Description("The highest the value, the more focused on the goal the ants behave (when taking food back to the hill)")]
		public float BringFoodGoal { get; set; }
		[CategoryAttribute("Ants Behaviors"),
		  Description("The highest the value, the more affected by the pheromones the ants will be (when taking food back to the hill)")]
		public float BringFoodPheromones { get; set; } 
		#endregion

		#endregion

		#endregion

		#region XML serialization

        private const string saveFile = @"XML\const.xml";

        /// <summary>
        /// To create the first xml
        /// </summary>
        public static void Main()
        {
            ConstantsHolder ch = new ConstantsHolder();
            ch.SaveXml();
        }

        /// <summary>
        /// saves current instance with all its attributes
        /// </summary>
        public void SaveXml()
        {
            StreamWriter W = null;
            try
            {
                W = new StreamWriter(saveFile);
                XmlSerializer S = new XmlSerializer(this.GetType());
                S.Serialize(W, this);
                W.Close();
            }
            catch//(Exception ex)
            {
                if(W != null)
                    W.Close();
				Console.WriteLine("unable to save");
                //System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// Opens the instance stored in the saveFile
        /// </summary>
        /// <returns>red class instance</returns>
        private static ConstantsHolder OpenXml()
        {
            StreamReader R = null;

            try
            {
                XmlSerializer S = new XmlSerializer(typeof(ConstantsHolder));
                R = new StreamReader(saveFile);
                ConstantsHolder ch = (ConstantsHolder)S.Deserialize(R);
                R.Close();

                return ch;
            }
            catch//(Exception ex)
            {
                if(R != null)
                    R.Close();
				Console.WriteLine("unable to load");
                //System.Windows.Forms.MessageBox.Show(ex.ToString());
                return null;
            }
        }

        #endregion
	}
}
