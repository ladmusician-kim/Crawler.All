using Newtonsoft.Json;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Crawler.DTO.ResponseDTO
{
    [ProtoContract]
    [JsonObject(MemberSerialization.OptIn)]
    public class EnvelopeDTO<T> where T : class
    {
        [JsonProperty]
        [ProtoMember(1)]
        public ResultTypeDTO ResultType { get; set; }

        [JsonProperty]
        [ProtoMember(2)]
        public T ReturnBody { get; set; }

        [JsonProperty]
        [ProtoMember(3)]
        public string ErrorMessage { get; set; }

        // Helper functions
        public bool IsSucceeded() { return ResultType == ResultTypeDTO.Success; }

        public bool IsFailed() { return ResultType != ResultTypeDTO.Success; }

        public bool IsResultType(int type) { return ResultType == (ResultTypeDTO)type; }

        [JsonIgnore]
        [ProtoIgnore]
        [IgnoreDataMember]
        public T SafeBody
        {
            get
            {
                if (this.IsSucceeded())
                    return ReturnBody;
                else
                    throw new NotImplementedException(ErrorMessage);
            }
        }
    }

    [ProtoContract]
    [JsonObject(MemberSerialization.OptIn)]
    public class EnvelopeDTO_struct<T> where T : struct
    {
        [JsonProperty]
        [ProtoMember(1)]
        public ResultTypeDTO ResultType { get; set; }
        [JsonProperty]
        [ProtoMember(2)]
        public T ReturnBody { get; set; }
        [JsonProperty]
        [ProtoMember(3)]
        public string ErrorMessage { get; set; }
        // Helper functions
        public bool IsSucceeded() { return ResultType == ResultTypeDTO.Success; }
        public bool IsFailed() { return ResultType != ResultTypeDTO.Success; }

        [JsonIgnore]
        [ProtoIgnore]
        public T SafeBody
        {
            get
            {
                if (this.IsSucceeded())
                    return ReturnBody;
                else
                    throw new NotImplementedException(ErrorMessage);
            }
        }
    }

    [ProtoContract]
    public enum ResultTypeDTO
    {
        Success = 200,
        Fail = 500,
        Exception = 501,
        IntegrityError = 502,
    }

    [ProtoContract]
    [ProtoInclude(1, typeof(EnvelopeDTO<GenericDummyDTO>))]
    [ProtoInclude(2, typeof(EnvelopeDTO<List<GenericDummyDTO>>))]
    public class GenericDummyDTO { }

    [ProtoContract]
    [ProtoInclude(1, typeof(EnvelopeDTO<GenericIDLabelDTO>))]
    [ProtoInclude(2, typeof(EnvelopeDTO<List<GenericIDLabelDTO>>))]
    public class GenericIDLabelDTO
    {
        [ProtoMember(1)]
        public int Id { get; set; }
        [ProtoMember(2)]
        public string Guid { get; set; }
        [ProtoMember(3)]
        public string Label { get; set; }
        [ProtoMember(4)]
        public DateTime? Paid { get; set; }
        [ProtoMember(5)]
        public int Count { get; set; }
    }

    [ProtoContract]
    [ProtoInclude(1, typeof(EnvelopeDTO<FileDTO>))]
    [ProtoInclude(2, typeof(EnvelopeDTO<List<FileDTO>>))]
    public class FileDTO
    {
        [ProtoMember(1)]
        public byte[] Filedata { get; set; }
        [ProtoMember(2)]
        public string Filename { get; set; }
        [ProtoMember(3)]
        public string Filetype { get; set; }
    }
}
