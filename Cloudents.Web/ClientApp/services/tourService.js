import { LanguageService } from './language/languageService'

  

   const toursOptions = {
        labels:{
          buttonSkip: LanguageService.getValueByKey('tour_label_skip'),
          buttonPrevious: LanguageService.getValueByKey('tour_label_previous'),
          buttonNext: LanguageService.getValueByKey('tour_label_next'),
          buttonStop: LanguageService.getValueByKey('tour_label_stop')
        }
      }
      // const israeliTourSteps = [
      const ilTours = {
        HWSteps:{
          desktop:[{
            target: ".ask-question-button",
           
            header: {
              title: LanguageService.getValueByKey('tour_add_question_button_title')
            },
            content: LanguageService.getValueByKey('tour_add_question_button_content'),
            params: {
              placement: "top",
              enableScrolling: true
            },
            offset: -650,
            arrowStyle: {
              borderTopColor: '#4870fd',
              borderBottomColor: '#4870fd',
            }
          },
          {
            target: "#tour_vote .question-input",
           
            header: {
              title: LanguageService.getValueByKey('tour_question_ticket_title')
            },
            content: LanguageService.getValueByKey('tour_question_ticket_content'),
            params: {
              placement: "top",
              enableScrolling: true
            },
            offset: -650,
            arrowStyle: {
              borderTopColor: '#4870fd',
              borderBottomColor: '#4870fd',
            }
          }],
          mobile:[
            {
            target: ".ask-question-button",
           
            header: {
              title: LanguageService.getValueByKey('tour_add_question_button_title')
            },
            content: LanguageService.getValueByKey('tour_add_question_button_content'),
            params: {
              placement: "top",
              enableScrolling: true
            },
            offset: -650,
            arrowStyle: {
              borderTopColor: '#4870fd',
              borderBottomColor: '#4870fd',
            }
          },
          {
            target: "#tour_vote .question-input",
           
            header: {
              title: LanguageService.getValueByKey('tour_question_ticket_title')
            },
            content: LanguageService.getValueByKey('tour_question_ticket_content'),
            params: {
              placement: "top",
              enableScrolling: true
            },
            offset: -650,
            arrowStyle: {
              borderTopColor: '#4870fd',
              borderBottomColor: '#4870fd',
            }
          }
        ]
        },
        StudyDocumentsSteps:{
          desktop:[
          {
            target: ".classes-holder .v-chip",
           
            header: {
              title: LanguageService.getValueByKey('tour_classes_title'),
            },
            content: LanguageService.getValueByKey('tour_classes_content'),
            params: {
              placement: "bottom",
              enableScrolling: true
            },
            offset: 0,
            arrowStyle: {
              borderTopColor: '#4870fd',
              borderBottomColor: '#4870fd',
            }
          },
          {
            target: ".upload-document-button",
           
            header: {
              title: LanguageService.getValueByKey('tour_upload_button_title'),
            },
            content: LanguageService.getValueByKey('tour_upload_button_content'),
            params: {
              placement: "top",
              enableScrolling: true
            },
            offset: -650,
            arrowStyle: {
              borderTopColor: '#4870fd',
              borderBottomColor: '#4870fd',
            }
          },
          {
            target: "#tour_vote .upvote-arrow",
           
            header: {
              title: LanguageService.getValueByKey('tour_vote_title')
            },
            content: LanguageService.getValueByKey('tour_vote_content'),
            params: {
              placement: "top",
              enableScrolling: true
            },
            offset: -650,
            arrowStyle: {
              borderTopColor: '#4870fd',
              borderBottomColor: '#4870fd',
            }
          },
          {
            target: ".v-list .leader-tile .leader-ammount",
           
            header: {
              title: LanguageService.getValueByKey('tour_sbl_currency_title'),
            },
            content: LanguageService.getValueByKey('tour_sbl_currency_content'),
            params: {
              placement: "bottom",
              enableScrolling: false
            },
            offset: -650,
            arrowStyle: {
              borderTopColor: '#4870fd',
              borderBottomColor: '#4870fd',
            }
          },],
          mobile:[
          {
            target: ".classes-holder .v-chip",
           
            header: {
              title: LanguageService.getValueByKey('tour_classes_title'),
            },
            content: LanguageService.getValueByKey('tour_classes_content'),
            params: {
              placement: "bottom",
              enableScrolling: true
            },
            offset: 0,
            arrowStyle: {
              borderTopColor: '#4870fd',
              borderBottomColor: '#4870fd',
            }
          },
          {
            target: ".upload-btn",
           
            header: {
              title: LanguageService.getValueByKey('tour_upload_button_title'),
            },
            content: LanguageService.getValueByKey('tour_upload_button_content'),
            params: {
              placement: "top",
              enableScrolling: true
            },
            offset: -650,
            arrowStyle: {
              borderTopColor: '#4870fd',
              borderBottomColor: '#4870fd',
            }
          },
          {
            target: "#tour_vote .upvote-arrow",
           
            header: {
              title: LanguageService.getValueByKey('tour_vote_title')
            },
            content: LanguageService.getValueByKey('tour_vote_content'),
            params: {
              placement: "top",
              enableScrolling: true
            },
            offset: -650,
            arrowStyle: {
              borderTopColor: '#4870fd',
              borderBottomColor: '#4870fd',
            }
          }
        ]
        }
      }
        
      
      const tourStseps = [
        {
          target: "#tour_vote .upvote-arrow",
         
          header: {
            title: LanguageService.getValueByKey('tour_vote_title')
          },
          content: LanguageService.getValueByKey('tour_vote_content'),
          params: {
            placement: "top",
            enableScrolling: true
          },
          offset: -450,
          arrowStyle: {
            borderTopColor: '#4870fd',
            borderBottomColor: '#4870fd',
          }
        }
      ]
export default {
    toursOptions,
    ilTours
}