﻿module TimingPoints

open JunUtils


type Tp =
    {
        time        : int;
        beatLength  : decimal;
        meter       : int;
        sampleSet   : int;
        sampleIndex : int;
        volume      : int;
        uninherited : bool;
        effects     : int;
    }

type TimingPoint = 
    | TimingPoint of Tp
    | Comment of string

let isTimingPoint            = function TimingPoint _ -> true          | _ -> false
let getTimingPoint           = function TimingPoint x -> x
let getTimingPointBeatLength = function TimingPoint x -> x.beatLength  | _ -> 0M

let isNotTimingPointComment hobj =
    match hobj with
    | Comment _ -> false
    | _ -> true

let removeTimingPointComments (objs:list<TimingPoint>) = (List.filter isNotTimingPointComment objs)

// timing point syntax:
// time,beatLength,meter,sampleSet,sampleIndex,volume,uninherited,effects
let tryParseTimingPoint line : TimingPoint option = 
    let values = parseCsv line
    match values with
    | [Decimal t; Decimal bl; Int m; Int ss; Int si; Int v; Bool ui; Int fx] ->
        Some(TimingPoint({
            time        = int t; // some maps save this as decimal...
            beatLength  = bl;
            meter       = m;
            sampleSet   = ss;
            sampleIndex = si;
            volume      = v;
            uninherited = ui;
            effects     = fx;
        }))
    | _ -> Some(Comment(line))
    //else Some(Comment(line))

let timingPointToString tp = 
    match tp with
    | TimingPoint tp -> sprintf "%d,%M,%d,%d,%d,%d,%d,%d" tp.time tp.beatLength tp.meter tp.sampleSet tp.sampleIndex tp.volume (if tp.uninherited then 1 else 0) tp.effects
    | Comment c -> c

let parseTimingPointSection : string list -> TimingPoint list = parseSectionUsing tryParseTimingPoint
