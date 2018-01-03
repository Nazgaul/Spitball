<template v-once>
    <general-page>
        <div slot="default" class="faq">
            <v-expansion-panel class="elevation-0" popout>
                <v-expansion-panel-content v-for="(question,i) in questions" :key="i">
                    <v-icon slot="actions" class="actions">sbf-chevron-down</v-icon>
                    <div slot="header">
                        <div>{{question.question}}</div>
                    </div>
                    <div><div class="pa-4" v-html="question.answer"></div></div>
                </v-expansion-panel-content>
            </v-expansion-panel>
        </div>
    </general-page>
</template>

<script>
import help from "../../services/satelliteService";
import generalPage from "./general.vue";
export default {
  components: { generalPage },
  data() {
    return {
      questions: null
    };
  },
  beforeRouteEnter(to, from, next) {
    help.getFaq().then(val => {
      console.log(val);
      next(vm => vm.setData(val.data));
    });
  },
  methods: {
    setData(questions) {
      this.questions = questions;
    }
  }
};
</script>

<style lang="less">
.faq {
  .expansion-panel {
    .expansion-panel__container {
      background-color: transparent;
      border-top-color: #302eb5;
      color: #302eb5;
      font-size: 18px;
      letter-spacing: -0.2px;
      .actions {
        color: #302eb5 !important;
      }
      .expansion-panel__body {
           background: #fff;
           color: #000000;
             line-height: 1.67;
   letter-spacing: -0.2px;
   font-size: 18px;
//   padding:24px;
           p {
               margin:0;
           }
      }
    }
  }
  .expansion-panel__container--active {
    .expansion-panel__header {
      background-color: #302eb5;
      color: #fff;
      .actions {
        color: #fff !important;
      }
    }
  }
}
</style>