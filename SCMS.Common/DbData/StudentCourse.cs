namespace SCMS.Common.DbData
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("StudentCourse")]
    public partial class StudentCourse
    {
        public int Id { get; set; }

        public long StudentId { get; set; }

        public int CourseId { get; set; }

        public byte? Score { get; set; }
        [JsonIgnore]
        public virtual Course Course { get; set; }
        [JsonIgnore]
        public virtual Person Person { get; set; }
    }
}
