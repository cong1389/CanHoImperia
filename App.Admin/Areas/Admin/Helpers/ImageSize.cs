using System.Configuration;

namespace App.Admin.Helpers
{
    public static class ImageSize
	{
		public static int HeightBigSize
		{
			get
			{
				if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["HeightBigSize"]))
				{
                    return int.Parse(ConfigurationManager.AppSettings["HeightDefaulSize"]);
                }
				return int.Parse(ConfigurationManager.AppSettings["HeightBigSize"]);
			}
		}

		public static int HeighthOrignalSize
		{
			get
			{
				if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["HeighthOrignalSize"]))
				{
                    return int.Parse(ConfigurationManager.AppSettings["HeightDefaulSize"]);
                }
				return int.Parse(ConfigurationManager.AppSettings["HeighthOrignalSize"]);
			}
		}
        
        public static int HeightMediumSize
		{
			get
			{
				if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["HeightMediumSize"]))
				{
                    return int.Parse(ConfigurationManager.AppSettings["HeightDefaulSize"]);
                }
				return int.Parse(ConfigurationManager.AppSettings["HeightMediumSize"]);
			}
		}

		public static int HeightNewsMediumSize
		{
			get
			{
				if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["HeightNewsSize"]))
				{
					return 400;
				}
				return int.Parse(ConfigurationManager.AppSettings["HeightNewsSize"]);
			}
		}
		public static int HeightNewsSize
		{
			get
			{
				if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["HeightNewsSize"]))
				{
					return 450;
				}
				return int.Parse(ConfigurationManager.AppSettings["HeightNewsSize"]);
			}
		}

	    public static int WithMediumNewsSize
	    {
	        get
	        {
	            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["WithNewsSize"]))
	            {
	                return int.Parse(ConfigurationManager.AppSettings["WithDefaultSize"]);
	            }
	            return int.Parse(ConfigurationManager.AppSettings["WithNewsSize"]);
	        }
	    }
	    public static int WithNewsSize
	    {
	        get
	        {
	            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["WithNewsSize"]))
	            {
	                return int.Parse(ConfigurationManager.AppSettings["WithDefaultSize"]);
	            }
	            return int.Parse(ConfigurationManager.AppSettings["WithNewsSize"]);
	        }
	    }
        
		public static int HeightSmallSize
		{
			get
			{
				if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["HeightSmallSize"]))
				{
                    return int.Parse(ConfigurationManager.AppSettings["HeightDefaulSize"]);
                }
				return int.Parse(ConfigurationManager.AppSettings["HeightSmallSize"]);
			}
		}

		public static int HeightThumbnailSize
		{
			get
			{
				if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["HeightThumbnailSize"]))
				{
                    return int.Parse(ConfigurationManager.AppSettings["HeightDefaulSize"]);
                }
				return int.Parse(ConfigurationManager.AppSettings["HeightThumbnailSize"]);
			}
		}

		public static int WithBigSize
		{
			get
			{
				if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["WithBigSize"]))
				{
					return int.Parse(ConfigurationManager.AppSettings["WithDefaultSize"]);
				}
				return int.Parse(ConfigurationManager.AppSettings["WithBigSize"]);
			}
		}
		public static int WithMediumSize
		{
			get
			{
				if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["WithMediumSize"]))
				{
                    return int.Parse(ConfigurationManager.AppSettings["WithDefaultSize"]);
                }
				return int.Parse(ConfigurationManager.AppSettings["WithMediumSize"]);
			}
		}
        
		public static int WithOrignalSize
		{
			get
			{
				if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["WithOrignalSize"]))
				{
                    return int.Parse(ConfigurationManager.AppSettings["WithDefaultSize"]);
                }
				return int.Parse(ConfigurationManager.AppSettings["WithOrignalSize"]);
			}
		}

        public static int FlowStepWithMediumSize
        {
            get
            {
                if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["FlowStepWithMediumSize"]))
                {
                    return int.Parse(ConfigurationManager.AppSettings["WithDefaultSize"]);
                }
                return int.Parse(ConfigurationManager.AppSettings["FlowStepWithMediumSize"]);
            }
        }
        public static int FlowStepHeightMediumSize
        {
            get
            {
                if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["FlowStep_HeightMediumSize"]))
                {
                    return int.Parse(ConfigurationManager.AppSettings["HeightDefaulSize"]);
                }
                return int.Parse(ConfigurationManager.AppSettings["FlowStep_HeightMediumSize"]);
            }
        }

        public static int ManufactureWithMediumSize
        {
            get
            {
                if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["Manufacture_WithMediumSize"]))
                {
                    return int.Parse(ConfigurationManager.AppSettings["WithDefaultSize"]);
                }
                return int.Parse(ConfigurationManager.AppSettings["Manufacture_WithMediumSize"]);
            }
        }
        public static int ManufactureHeightMediumSize
        {
            get
            {
                if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["Manufacture_HeightMediumSize"]))
                {
                    return int.Parse(ConfigurationManager.AppSettings["HeightDefaulSize"]);
                }
                return int.Parse(ConfigurationManager.AppSettings["Manufacture_HeightMediumSize"]);
            }
        }
        
        public static int PaymentMethodWithMediumSize
        {
            get
            {
                if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["PaymentMethod_WithMediumSize"]))
                {
                    return int.Parse(ConfigurationManager.AppSettings["WithDefaultSize"]);
                }
                return int.Parse(ConfigurationManager.AppSettings["PaymentMethod_WithMediumSize"]);
            }
        }
        public static int PaymentMethodHeightMediumSize
        {
            get
            {
                if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["PaymentMethod_HeightMediumSize"]))
                {
                    return int.Parse(ConfigurationManager.AppSettings["HeightDefaulSize"]);
                }
                return int.Parse(ConfigurationManager.AppSettings["PaymentMethod_HeightMediumSize"]);
            }
        }

	    public static int HeightPostRelativeSize
	    {
	        get
	        {
	            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["HeightPostRelativeSize"]))
	            {
	                return 384;
	            }
	            return int.Parse(ConfigurationManager.AppSettings["HeightPostRelativeSize"]);
	        }
	    }
        public static int WithPostRelativeSize
		{
			get
			{
				if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["WithPostRelativeSize"]))
				{
                    return int.Parse(ConfigurationManager.AppSettings["WithDefaultSize"]);
                }
				return int.Parse(ConfigurationManager.AppSettings["WithPostRelativeSize"]);
			}
		}

		public static int WithSmallSize
		{
			get
			{
				if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["WithSmallSize"]))
				{
                    return int.Parse(ConfigurationManager.AppSettings["WithDefaultSize"]);
                }
				return int.Parse(ConfigurationManager.AppSettings["WithSmallSize"]);
			}
		}
		public static int WithThumbnailSize
		{
			get
			{
				if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["WithThumbnailSize"]))
				{
                    return int.Parse(ConfigurationManager.AppSettings["WithDefaultSize"]);
                }
				return int.Parse(ConfigurationManager.AppSettings["WithThumbnailSize"]);
			}
		}

	    public static int MenuWithBigSize
        {
	        get
	        {
	            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["MenuWithBigSize"]))
	            {
	                return int.Parse(ConfigurationManager.AppSettings["WithDefaultSize"]);
                }
	            return int.Parse(ConfigurationManager.AppSettings["MenuWithBigSize"]);
	        }
	    }
	    public static int MenuHeightBigSize
        {
	        get
	        {
	            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["MenuHeightBigSize"]))
	            {
	                return int.Parse(ConfigurationManager.AppSettings["HeightDefaulSize"]);
                }
	            return int.Parse(ConfigurationManager.AppSettings["MenuHeightBigSize"]);
	        }
	    }
	    public static int MenuWithMediumSize
        {
	        get
	        {
	            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["MenuWithMediumSize"]))
	            {
	                return int.Parse(ConfigurationManager.AppSettings["WithDefaultSize"]);
	            }
	            return int.Parse(ConfigurationManager.AppSettings["MenuWithMediumSize"]);
	        }
	    }
	    public static int MenuHeightMediumSize
        {
	        get
	        {
	            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["MenuHeightMediumSize"]))
	            {
	                return int.Parse(ConfigurationManager.AppSettings["HeightDefaulSize"]);
	            }
	            return int.Parse(ConfigurationManager.AppSettings["MenuHeightMediumSize"]);
	        }
	    }
	    public static int MenuWithSmallSize
	    {
	        get
	        {
	            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["MenuWithSmallSize"]))
	            {
	                return int.Parse(ConfigurationManager.AppSettings["WithDefaultSize"]);
	            }
	            return int.Parse(ConfigurationManager.AppSettings["MenuWithSmallSize"]);
	        }
	    }
	    public static int MenuHeightSmallSize
	    {
	        get
	        {
	            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["MenuHeightSmallSize"]))
	            {
	                return int.Parse(ConfigurationManager.AppSettings["HeightDefaulSize"]);
	            }
	            return int.Parse(ConfigurationManager.AppSettings["MenuHeightSmallSize"]);
	        }
	    }

	    public static int BannerWithBigSize
	    {
	        get
	        {
	            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["BannerWithBigSize"]))
	            {
	                return int.Parse(ConfigurationManager.AppSettings["WithDefaultSize"]);
	            }
	            return int.Parse(ConfigurationManager.AppSettings["BannerWithBigSize"]);
	        }
	    }
	    public static int BannerHeightBigSize
	    {
	        get
	        {
	            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["BannerHeightBigSize"]))
	            {
	                return int.Parse(ConfigurationManager.AppSettings["HeightDefaulSize"]);
	            }
	            return int.Parse(ConfigurationManager.AppSettings["BannerHeightBigSize"]);
	        }
	    }

	    public static int SlideShowWithBigSize
	    {
	        get
	        {
	            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["SlideShowWithBigSize"]))
	            {
	                return int.Parse(ConfigurationManager.AppSettings["WithDefaultSize"]);
	            }
	            return int.Parse(ConfigurationManager.AppSettings["SlideShowWithBigSize"]);
	        }
	    }
	    public static int SlideShowHeightBigSize
	    {
	        get
	        {
	            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["SlideShowHeightBigSize"]))
	            {
	                return int.Parse(ConfigurationManager.AppSettings["HeightDefaulSize"]);
	            }
	            return int.Parse(ConfigurationManager.AppSettings["SlideShowHeightBigSize"]);
	        }
	    }

	    public static int StaticContentWithBigSize
	    {
	        get
	        {
	            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["StaticContentWithBigSize"]))
	            {
	                return int.Parse(ConfigurationManager.AppSettings["WithDefaultSize"]);
	            }
	            return int.Parse(ConfigurationManager.AppSettings["StaticContentWithBigSize"]);
	        }
	    }
	    public static int StaticContentHeightBigSize
	    {
	        get
	        {
	            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["StaticContentHeightBigSize"]))
	            {
	                return int.Parse(ConfigurationManager.AppSettings["HeightDefaulSize"]);
	            }
	            return int.Parse(ConfigurationManager.AppSettings["StaticContentHeightBigSize"]);
	        }
	    }

        public static int NewsWithBigSize
        {
            get
            {
                if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["NewsWithBigSize"]))
                {
                    return int.Parse(ConfigurationManager.AppSettings["WithDefaultSize"]);
                }
                return int.Parse(ConfigurationManager.AppSettings["NewsWithBigSize"]);
            }
        }
        public static int NewsHeightBigSize
        {
            get
            {
                if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["NewsHeightBigSize"]))
                {
                    return int.Parse(ConfigurationManager.AppSettings["HeightDefaulSize"]);
                }
                return int.Parse(ConfigurationManager.AppSettings["NewsHeightBigSize"]);
            }
        }
        public static int NewsWithMediumSize
        {
            get
            {
                if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["NewsWithMediumSize"]))
                {
                    return int.Parse(ConfigurationManager.AppSettings["WithDefaultSize"]);
                }
                return int.Parse(ConfigurationManager.AppSettings["NewsWithMediumSize"]);
            }
        }
        public static int NewsHeightMediumSize
        {
            get
            {
                if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["NewsHeightMediumSize"]))
                {
                    return int.Parse(ConfigurationManager.AppSettings["HeightDefaulSize"]);
                }
                return int.Parse(ConfigurationManager.AppSettings["NewsHeightMediumSize"]);
            }
        }
        public static int NewsWithSmallSize
        {
            get
            {
                if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["NewsWithSmallSize"]))
                {
                    return int.Parse(ConfigurationManager.AppSettings["WithDefaultSize"]);
                }
                return int.Parse(ConfigurationManager.AppSettings["NewsWithSmallSize"]);
            }
        }
        public static int NewsHeightSmallSize
        {
            get
            {
                if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["NewsHeightSmallSize"]))
                {
                    return int.Parse(ConfigurationManager.AppSettings["HeightDefaulSize"]);
                }
                return int.Parse(ConfigurationManager.AppSettings["NewsHeightSmallSize"]);
            }
        }
    }
}