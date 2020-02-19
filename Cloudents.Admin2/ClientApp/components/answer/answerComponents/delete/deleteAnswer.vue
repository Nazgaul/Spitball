<template>
    <div class="">
        <h1>Delete Answer</h1>
        <div class="wrap">
            <div class="info">
                <h3>To Delete multiple answers, enter the answer Id's seperated with a comma (See example).</h3>
                <h4 v-if="infoSuccess">{{infoSuccess}}</h4>
                <h4 v-else-if="infoError">{{infoError}}</h4>
            </div>
            <div class="input-wrap">
                <v-text-field solo class="id-input" type="text" v-model="answersIdString"
                              placeholder="example: 1245,6689,1123"/>
                <v-btn round color="red" @click="deleteByIds" class="btn-danger">Delete</v-btn>
            </div>
        </div>
    </div>

</template>
<script>

    import { deleteAnswer } from './deleteAnswerService'

    export default {
        data() {
            return {
                answersIds: [],
                answersIdString: '',
                infoSuccess: '',
                infoError: ''
            }
        },
        methods: {
            deleteByIds() {
                if (this.answersIdString.length > 0) {
                    this.answersIds = this.answersIdString.split(',');
                    let numberArr = [];
                    this.answersIds.forEach(id => {
                        let num = id;
                        if (!!num) {
                            return numberArr.push(num);
                        }

                    })
                    deleteAnswer(numberArr)
                        .then(resp => {

                            this.$toaster.success(`Answer were deleted: ${this.answersIdString}`);
                            this.answersIdString = '';
                            this.answersIds = [];
                            },
                            (error) => {
                                this.$toaster.error('Something went wrong');
                                console.log('component delete error', error)
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

</style>