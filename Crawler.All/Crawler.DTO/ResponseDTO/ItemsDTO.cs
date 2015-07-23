using ProtoBuf;
using System.Collections.Generic;

namespace Crawler.DTO.ResponseDTO
{
    [ProtoContract]
    public class ItemsDTO<T>
    {
        [ProtoMember(1)]
        public int PerPage { get; set; }
        [ProtoMember(2)]
        public int RowCount { get; set; }
        [ProtoMember(3)]
        public List<T> Items { get; set; }
    }
    [ProtoContract]
    [ProtoInclude(1, typeof(EnvelopeDTO<BooleanDTO>))]
    [ProtoInclude(2, typeof(EnvelopeDTO<List<BooleanDTO>>))]
    [ProtoInclude(3, typeof(EnvelopeDTO<ItemsDTO<BooleanDTO>>))]
    public class BooleanDTO
    {
        [ProtoMember(1)]
        public bool Result { get; set; }
    }
}