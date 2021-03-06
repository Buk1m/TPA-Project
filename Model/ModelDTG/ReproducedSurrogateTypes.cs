﻿using System.Collections.Generic;

namespace Model.ModelDTG
{
    public class ReproducedSurrogateTypes : Dictionary<string, TypeMetadata>
    {
        private static ReproducedSurrogateTypes _reproducedSurrogateTypes = null;

        private ReproducedSurrogateTypes()
        {
        }

        public static ReproducedSurrogateTypes Instance =>
            _reproducedSurrogateTypes ?? ( _reproducedSurrogateTypes = new ReproducedSurrogateTypes() );
    }
}