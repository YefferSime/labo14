﻿namespace lab12.Requests
{
    public class StudentInsertRequestV6
    {
        public int IdGrade { get; set; }
        public List<StudentRequestV1> Students { get; set; }  // Usamos el modelo StudentRequestV1 para representar a cada estudiante
    }
}