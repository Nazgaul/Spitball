<template>
    <div class="delete-question-container">
        <h1 align="center">Delete Question</h1>
        <div class="wrap">
            <div class="info">
                <h3>To Delete multiple questions, enter the question Id's seperated with a comma (See example).</h3>
                <h4 v-if="infoSuccess">{{infoSuccess}}</h4>
                <h4 v-else-if="infoError">{{infoError}}</h4>
            </div>
            <div class="input-wrap">
                <v-text-field solo class="id-input" type="text" v-model="questionsIdString"
                              placeholder="example: 1245,6689,1123"/>
                <v-btn color="red" :loading="loading" @click="deleteByIds" class="btn-danger">Delete</v-btn>
            </div>
        </div>
    </div>

</template>
<script>

    import { deleteQuestion } from './deleteQuestionService'

    export default {
        data() {
            return {
                questionsIds: [],
                questionsIdString: '',
                infoSuccess: '',
                infoError: '',
                loading: false
            }
        },
        methods: {
            deleteByIds() {
                this.loading = true;
                if (this.questionsIdString.length > 0) {
                    this.questionsIds = this.questionsIdString.split(',');
                    let numberArr = [];
                    this.questionsIds.forEach(id => {
                        let num = parseInt(id.trim());
                        if (!!num) {
                            return numberArr.push(num);
                        }

                    })
                    deleteQuestion(numberArr)
                        .then(resp => {

                                this.$toaster.success(`Questions were deleted: ${this.questionsIdString}`);
                                this.questionsIdString = '';
                                this.questionsIds = [];
                                this.loading = false;


                            },
                            (error) => {

                                this.$toaster.error('Something went wrong');
                                console.log('component delete error', error)
                                this.loading = false;
                            }
                        )
                }
            },

        },
        created() {

        }
    }
</script>

<style lang="less" scoped>
    .delete-question-container{
        margin: 0 auto;
    .input-wrap {
            display: flex;
            flex-direction: column;
            align-items: center;
            justify-content: center;
            .id-input {
                width: 100%;
                max-width: 400px;
                border-radius: 25px;
                border: none;
                outline: none;
                padding-top: 12px;
                /*padding-left: 10px;*/
            }
        }
    }
    

</style>