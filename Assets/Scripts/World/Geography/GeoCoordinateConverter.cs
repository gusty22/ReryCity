using System;
using UnityEngine;

namespace ProjetoGrau2.World.Geography
{
    /// <summary>
    /// Utilitário matemático de alta precisão para conversão bidirecional entre coordenadas
    /// geográficas (WGS84 Latitude/Longitude) e coordenadas planas locais no Unity (X, Z).
    /// Utiliza projeção planar equidistante tangencial à latitude média para eliminar distorções
    /// e contornar limitações de ponto flutuante em jogos de mundo aberto.
    /// </summary>
    public static class GeoCoordinateConverter
    {
        public const double EarthRadiusMeters = 6378137.0; // Raio equatorial WGS84
        public const double DegToRad = Math.PI / 180.0;
        public const double RadToDeg = 180.0 / Math.PI;

        /// <summary>
        /// Converte coordenadas geográficas (latitude, longitude) para coordenadas cartesianas no Unity (X, Z)
        /// relativas a um ponto de origem geográfico.
        /// X = Leste (+East), Z = Norte (+North).
        /// </summary>
        public static Vector3 GeoToWorld(double latitude, double longitude, double altitude, GeoOriginConfig origin)
        {
            if (origin == null)
            {
                Debug.LogWarning("[GeoCoordinateConverter] Origem geográfica nula, assumindo coordenadas zeradas.");
                return Vector3.zero;
            }

            double originLatRad = origin.latitude * DegToRad;
            double cosOriginLat = Math.Cos(originLatRad);

            double dLat = (latitude - origin.latitude) * DegToRad;
            double dLon = (longitude - origin.longitude) * DegToRad;

            double localX = dLon * EarthRadiusMeters * cosOriginLat * origin.scaleFactor;
            double localZ = dLat * EarthRadiusMeters * origin.scaleFactor;
            double localY = (altitude - origin.altitudeMeters) * origin.scaleFactor;

            // Rotação de orientação em relação ao Norte geográfico se configurada
            if (Math.Abs(origin.headingRotationDegrees) > 0.001)
            {
                double rotRad = -origin.headingRotationDegrees * DegToRad;
                double cosRot = Math.Cos(rotRad);
                double sinRot = Math.Sin(rotRad);

                double rotatedX = localX * cosRot - localZ * sinRot;
                double rotatedZ = localX * sinRot + localZ * cosRot;

                localX = rotatedX;
                localZ = rotatedZ;
            }

            return new Vector3((float)localX, (float)localY, (float)localZ);
        }

        /// <summary>
        /// Converte coordenadas locais no Unity (X, Z) de volta para geográficas (latitude, longitude)
        /// com base na origem configurada.
        /// </summary>
        public static void WorldToGeo(Vector3 worldPos, GeoOriginConfig origin, out double latitude, out double longitude, out double altitude)
        {
            if (origin == null)
            {
                latitude = 0;
                longitude = 0;
                altitude = 0;
                return;
            }

            double localX = worldPos.x;
            double localZ = worldPos.z;

            // Reverter rotação
            if (Math.Abs(origin.headingRotationDegrees) > 0.001)
            {
                double rotRad = origin.headingRotationDegrees * DegToRad;
                double cosRot = Math.Cos(rotRad);
                double sinRot = Math.Sin(rotRad);

                double unrotX = localX * cosRot - localZ * sinRot;
                double unrotZ = localX * sinRot + localZ * cosRot;

                localX = unrotX;
                localZ = unrotZ;
            }

            double originLatRad = origin.latitude * DegToRad;
            double cosOriginLat = Math.Cos(originLatRad);

            double dLat = (localZ / origin.scaleFactor) / EarthRadiusMeters;
            double dLon = (localX / origin.scaleFactor) / (EarthRadiusMeters * cosOriginLat);

            latitude = origin.latitude + (dLat * RadToDeg);
            longitude = origin.longitude + (dLon * RadToDeg);
            altitude = origin.altitudeMeters + (worldPos.y / origin.scaleFactor);
        }
    }
}
