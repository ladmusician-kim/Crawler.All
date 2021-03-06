﻿using Crawler.DTO.ResponseDTO;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crawler.DTO.RequestDTO
{
    [ProtoContract(ImplicitFields=ImplicitFields.AllPublic)]
    public class SnapshotCreateRequestDTO
    {
        public SnapshotDTO Snapshot { get; set; }
    }

    public class SnapshotUpdateRequestDTO
    {
        public SnapshotDTO Snapshot { get; set; }
    }

      [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class SnapshotGetbyIdRequestDTO
    {
        public int? SnapshotId { get; set; }
    }

    public class SnapshotSaveRequestDTO
    {
        public SnapshotDTO Snapshot { get; set; }
        public DateTime? TimeSlot { get; set; }
    }

      [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class SnapshotGetbyBoardIdRequestDTO
    {
        public int? For_BoardId { get; set; }
    }

    public class SnapshotListGetbyBoardIdRequestDTO
    {
        public int? For_BoardId { get; set; }
    }
    public class SnapshotListGetbyDataAndBoardIdRequestDTO
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<int> boardIdList { get; set; }
    }
}