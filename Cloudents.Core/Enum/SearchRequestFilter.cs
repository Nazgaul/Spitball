namespace Cloudents.Core.Enum
{
    public enum TutorRequestFilter
    {
        None,
        Online,
        InPerson
    }

    public enum PlacesRequestFilter
    {
        None,
        OpenNow
    }

    public enum JobRequestFilter
    {
        None,
        Paid
    }

    //tutor - filter = FilterSortDto(filters: [.all,.online,.inPerson], sortArr: [.relevance, .price,.distance,.rating])
    /*flashcard filter = FilterSortDto(filters: [Filterable(title: .all),sources], sortArr: .relevance,.date)
    /*document filter = FilterSortDto(filters: [Filterable(title: .all),sources], sortArr: .relevance,.date)
     * purchase   filter = FilterSortDto(filters: [            .all,.openNow            ], sortArr: [.distance])
     * jobs  filter = FilterSortDto(filters: [Filterable(title: .all),jobType,Filterable(title: .paid)], sortArr: [.relevance, .date,.distance])
       book detail  let filter = FilterSortDto(filters: [.all, .new, .rent, .eBook], sortArr: [.price])
     */
}