import { LanguageService } from './language/languageService'

   const toursOptions = {
        startTimeout: 3000,
        labels:{
          buttonSkip: LanguageService.getValueByKey('tour_label_skip'),
          buttonPrevious: LanguageService.getValueByKey('tour_label_previous'),
          buttonNext: LanguageService.getValueByKey('tour_label_next'),
          buttonStop: LanguageService.getValueByKey('tour_label_stop')
        }
      }
      const tourSteps = [
        {
          target: "#tour_vote .upvote-arrow",
         
          header: {
            title: LanguageService.getValueByKey('tour_vote_title')
          },
          content: LanguageService.getValueByKey('tour_vote_content'),
          params: {
            placement: "top"
          },
          arrowStyle: {
            borderTopColor: '#4870fd',
            borderBottomColor: '#4870fd',
          }
        }
      ]
export default {
    toursOptions,
    tourSteps
}