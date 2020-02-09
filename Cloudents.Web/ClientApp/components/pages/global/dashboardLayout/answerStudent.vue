<template>
    <v-row class="answerStudent mt-2 mb-4 mx-0" dense>
        <v-col cols="12" class="answerTitle text-center mb-4">
            Answer student questions
        </v-col>

        <v-col class="answerList pr-3">
            <div class="answerItem pb-3"  v-for="(answer, index) in answers" :key="index">
                <div class="fakeImage"></div>
                <div class="middle pl-4 pb-4">
                    <div class="top d-flex justify-space-between">
                        <div class="name mb-1 pr-4 text-truncate">{{answer.name}}</div>
                        <div class="date">{{answer.date}}</div>
                    </div>
                    <div class="text">{{answer.text}}</div>
                </div>
            </div>
        </v-col>
    </v-row>
</template>
<script>
export default {
  name: "answerStudent",
  data: () => ({
    answers: [
      {
        image: '',
        name: 'Jone smith',
        text: 'How can i get a conversation going or as a fun way to pass the time with friends or loved on How can i get a conversation going or as a fun way to pass the time with friends or loved on How can i get a conversation going or as a fun way to pass the time with friends or loved on',
        date: new Date().toLocaleDateString("en-IL",{ year: 'numeric', month: 'short', day: 'numeric' })
      },
      {
        image: '',
        name: 'Jone smith',
        text: 'How can i get a conversation going or as a fun way to pass the time with friends or loved on',
        date: new Date().toLocaleDateString("en-IL",{ year: 'numeric', month: 'short', day: 'numeric' })
      },
      {
        image: '',
        name: 'Jone smith',
        text: 'How can i get a conversation going or as a fun way to pass the time with friends or loved on How can i get a conversation going or as a fun way to pass the time with friends or loved on How can i get a conversation going or as a fun way to pass the time with friends or loved on',
        date: new Date().toLocaleDateString("en-IL",{ year: 'numeric', month: 'short', day: 'numeric' })
      },
      {
        image: '',
        name: 'Jone smith',
        text: 'How can i get a conversation going or as a fun way to pass the time with friends or loved on How can i get a conversation going or as a fun way to pass the time with friends or loved on How can i get a conversation going or as a fun way to pass the time with friends or loved on',
        date: new Date().toLocaleDateString("en-IL",{ year: 'numeric', month: 'short', day: 'numeric' })
      },
      {
        image: '',
        name: 'Jone smith',
        text: 'How can i get a conversation going or as a fun way to pass the time with friends or loved on How can i get a conversation going or as a fun way to pass the time with friends or loved on How can i get a conversation going or as a fun way to pass the time with friends or loved on',
        date: new Date().toLocaleDateString("en-IL",{ year: 'numeric', month: 'short', day: 'numeric' })
      },
      {
        image: '',
        name: 'Jone smith',
        text: 'How can i get a conversation going or as a fun way to pass the time with friends or loved on How can i get a conversation going or as a fun way to pass the time with friends or loved on How can i get a conversation going or as a fun way to pass the time with friends or loved on',
        date: new Date().toLocaleDateString("en-IL",{ year: 'numeric', month: 'short', day: 'numeric' })
      },
      {
        image: '',
        name: 'Jone smith',
        text: 'How can i get a conversation going or as a fun way to pass the time with friends or loved on How can i get a conversation going or as a fun way to pass the time with friends or loved on How can i get a conversation going or as a fun way to pass the time with friends or loved on',
        date: new Date().toLocaleDateString("en-IL",{ year: 'numeric', month: 'short', day: 'numeric' })
      },
      {
        image: '',
        name: 'Jone smith',
        text: 'How can i get a conversation going or as a fun way to pass the time with friends or loved on How can i get a conversation going or as a fun way to pass the time with friends or loved on How can i get a conversation going or as a fun way to pass the time with friends or loved on',
        date: new Date().toLocaleDateString("en-IL",{ year: 'numeric', month: 'short', day: 'numeric' })
      },
      {
        image: '',
        name: 'Jone smith',
        text: 'How can i get a conversation going or as a fun way to pass the time with friends or loved on How can i get a conversation going or as a fun way to pass the time with friends or loved on How can i get a conversation going or as a fun way to pass the time with friends or loved on',
        date: new Date().toLocaleDateString("en-IL",{ year: 'numeric', month: 'short', day: 'numeric' })
      },
    ]
  })
}
</script>
<style lang="less">
@color: #43425d;
.answerStudent {
  padding: 12px 16px;
  background: white;
  box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.15);
  border-radius: 8px;

  @media (max-width: 599px) {
    box-shadow: none;
  }

  .answerTitle {
    color: @color;
    font-weight: 600;
    font-size: 18px;
  }

  .answerList {
    overflow-y: scroll;
    max-height: 260px;
    .answerItem {
      display: flex;
      .fakeImage {
        width: 42px;
        height: 42px;
        border-radius: 50%;
        background: blue;
      }
      .middle {
        border-bottom: 1px solid #dddddd;
        color: @color;
        min-width: 0;
        width: 100%;
        .top {

          .name {
            color: @color;
            font-weight: bold;
          }
          .date {
            color: #a0a0a0;
            font-size: 12px;
            flex-shrink: 0;
          }
        }
        .text {
          .giveMeEllipsis(2, 22);
        }
      }
      &:last-child .middle {
        border-bottom: none;
      }
    }
  }
}
.giveMeEllipsis(@lines, @line-height,@type:px) {
  overflow: hidden;
  text-overflow: ellipsis;
  display: -webkit-box;
  -webkit-box-orient: vertical;
  -webkit-line-clamp: @lines; /* number of lines to show */
  line-height: unit(@line-height, @type); /* fallback */
  max-height: unit((@lines*@line-height), @type); /* fallback */
}
</style>
