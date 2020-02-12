<template>
    <v-row class="answerStudent mt-2 mb-4 mx-0" dense>
        <v-col cols="12" class="answerTitle text-center mb-4">
            {{$t('dashboardTeacher_answer_title')}}
        </v-col>

        <v-col class="answerList pr-3">
            <div class="answerItem pb-3"  v-for="(answer, index) in answers" :key="index">
                <UserAvatar :size="'34'" :user-name="answer.user.name" :user-id="answer.user.id" :userImageUrl="answer.user.image" /> 
                <div class="middle pl-4 pb-4">
                    <div class="top d-flex justify-space-between">
                        <div class="name mb-1 pr-4 text-truncate">{{answer.user.name}}</div>
                        <div class="date">{{ $d(new Date(answer.dateTime), 'short') }}</div>
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
    answers: []
  }),
  methods: {
    getStudentsAnswers() {
      let self = this;
      this.$store.dispatch('updateStudentsAnswersQuestion').then(({data}) => {
        self.answers = data
      }).catch(ex => {
        self.$appInsights.trackException({exception: new Error(ex)});
      })
    }
  },
  created() {
    this.getStudentsAnswers()
  }
}
</script>
<style lang="less">
@import '../../../../styles/mixin.less';
@import '../../../../styles/colors.less';
.answerStudent {
  padding: 12px 16px;
  background: white;
  box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.15);
  border-radius: 8px;

  @media (max-width: @screen-xs) {
    box-shadow: none;
  }

  .answerTitle {
    color: @global-purple;
    font-weight: 600;
    font-size: 18px;
  }

  .answerList {
    overflow-y: scroll;
    max-height: 260px;
    .answerItem {
      display: flex;
      .middle {
        border-bottom: 1px solid #dddddd;
        color: @global-purple;
        min-width: 0;
        width: 100%;
        min-height: 84px;
        .top {

          .name {
            color: @global-purple;
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
</style>
