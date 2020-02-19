<template>
    <div class="add-question-container">
        <h1>Add A question</h1>
        <v-form  ref="form">
<span class="select-subject">First <b>Select</b> A subject:</span>

        <div class="inputs-wrap">
            <v-select attach=""
                      @change="optionChanegd"
                      v-model="selectedSubject"
                      class="select-subject-input"
                      solo
                      :items="subjects"
                      item-value="id"
                      item-text="subject"
                      label="Select Subject"

            ></v-select>
            <div class="text-area-container" v-if="showTextArea">
                <v-textarea solo v-model="subjectContent" placeholder="Content of text..." cols="30" rows="10"></v-textarea>
            </div>
            <div class="select-type-container" v-if="showTextArea">
                <v-select attach=""
                          class="select-country-input"
                          solo
                          v-model="country"
                          :items="countryList"
                          :item-value="country"
                          label="Select country"
                ></v-select>
                <div class="price-container">
                    <div >
                        <v-text-field solo class="question-price" :rules="[rules.required]"  placeholder="University name" :type="'text'" v-model="uni" />
                    </div>
                </div>
                <div class="price-container" v-if="uni">
                    <div>
                        <v-text-field solo class="question-price" :rules="[rules.required]" placeholder="Course name" :type="'text'" v-model="course" />
                    </div>
                </div>
            </div>
        </div>
        <div class="upload-container" v-if="showTextArea">
            <file-upload
                    ref="upload"
                    v-model="files"
                    post-action="/api/AdminQuestion/upload"
                    @input-file="inputFile"
                    @input-filter="inputFilter"
                    :multiple="true"
                    :maximum="4"
                    :extensions="['jpeg', 'jpe', 'jpg', 'gif', 'png', 'webp']"
            >
                <v-btn  class="btn-upload">Upload File</v-btn>
            </file-upload>
            <ul>
                <li style="list-style: none;" v-for="(file, index) in files" :key="index">
                    {{file.name}} <span v-if="file.error">- Error: {{file.error}}</span>
                    <span v-if="file.success">Success: {{file.success}}</span>
                </li>
            </ul>
        </div>
        <div class="price-container" v-if="showTextArea">
            <v-btn v-if="!showPriceSetter"  @click="setPrice">Set A Price</v-btn>
            <div v-if="showPriceSetter">
                <v-text-field solo class="question-price" type="number" v-model="questionPrice" min="1"/>
            </div>
        </div>

        <div class="add-container" v-if="showPriceSetter && uni" >
            <v-btn   :loading="loading" class="btn-add" @click="addQ">Add</v-btn>
            <v-btn flat  @click="$refs.form.reset()">Clear</v-btn>
        </div>
        </v-form>
    </div>
</template>

<script>
    import { getSubjectList, addQuestion } from './addQuestionService'

    export default {
        data() {
            return {
                subjects: [],
                selectedSubject: '',
                subjectContent: '',
                showTextArea: false,
                questionPrice: 1,
                showPriceSetter: false,
                showCourseSetter: false,
                files: [],
                filesNames: [],
                country: "Us",
                countryList: ['Us', 'Il', 'In'],
                uni: '',
                course: '',
                loading: false,
                rules: {
                    required: value => !!value || 'This field is required',
                    // minimum: value => value.length >= 2 || 'Min 2 characters'
                }
            }
        },
        methods: {
            optionChanegd: function () {
                this.showTextArea = true;
                console.log(this.selectedSubject)
            },
            resetData(){
                this.selectedSubject= '';
                this.subjectContent= '';
                this.showTextArea= false;
                this.questionPrice= 1;
                this.showPriceSetter= false;
                this.files= [];
                this.filesNames= [];
                this.country= "Us";
                this.course = '';
                this.uni = '';
            },
            setPrice: function () {
                this.showPriceSetter = true;
            },
            setUni(){
                this.showCourseSetter = true;
            },
            addQ: function () {
                this.loading = true;
                if (this.questionPrice < 1) {
                    this.$toaster.error("Error: Price must be above 1");
                    return;
                }
                if (this.subjectContent === '') {
                    this.$toaster.error("Error: No Content");
                    return;
                }
                let uploads = [];
                if (this.files.length > 0) {
                    this.files.forEach(file => {
                        if (!!file.response.file) {
                            uploads.push(file.response.file)
                        }
                    })
                }
                addQuestion(this.selectedSubject, this.subjectContent, this.questionPrice, this.country, this.uni, this.course, uploads).then(() => {
                    this.resetData();
                    this.$toaster.success("Success on Adding Question");
                    this.loading= false;
                }, (err) => {
                    console.log(err);
                    this.$toaster.error("Error: Failed to Add question");
                    this.loading= false;
                })
            },
            inputFile(newFile, oldFile) {
                // Automatic upload
                if (Boolean(newFile) !== Boolean(oldFile) || oldFile.error !== newFile.error) {
                    if (!this.$refs.upload.active) {
                        this.$refs.upload.active = true
                    }
                }
                if (!!oldFile) {
                    // Upload error
                    if (newFile.error !== oldFile.error) {
                        console.log('error', newFile.error, newFile)
                    }

                    // Uploaded successfully
                    if (newFile.success !== oldFile.success) {
                        console.log('success', newFile.success, newFile)
                    }
                }
            },
            inputFilter: function (newFile, oldFile, prevent) {
                if (newFile && !oldFile) {
                    // Filter non-image file
                    if (!/\.(jpeg|jpe|jpg|gif|png|webp)$/i.test(newFile.name)) {
                        return prevent()
                    }
                }

                // Create a blob field
                newFile.blob = ''
                let URL = window.URL || window.webkitURL
                if (URL && URL.createObjectURL) {
                    newFile.blob = URL.createObjectURL(newFile.file)
                }
            },
        },
        created() {
            getSubjectList().then((responseSubjects) => {
                this.subjects = responseSubjects
            })
        }
    }
</script>

<style lang="less" scoped>
    .add-question-container {
            margin: 0 auto;
        span.select-subject{
            font-size: 16px;
            margin-top: 25px;
            display: block;
        }
        .inputs-wrap {
            padding-top: 5px;
            display: flex;
            flex-direction: column;
            align-items: center;
            justify-content: center;
        }
        .select-subject-input, select-country-input {
            width: 345px;
        }
        .text-area-container, .select-type-container {
            width: 345px;
            textarea {
                margin-top: 25px;
                border: none;
                background-color: rgb(180, 224, 165);
                color: rgb(124, 121, 121);
                outline: none;
                width: 500px;
                height: 150px;
                padding: 10px;
            }
        }
        .price-container, .add-container, .upload-container {
            display: flex;
            flex-direction: column;
            align-items: center;
            justify-content: center;
            margin-top: 25px;
            .question-price {
            width: 345px;
            }
        }
        .select-type-container {
            .select-type {
                border: none;
                border-radius: 25px;
                height: 25px;
                margin-top: 10px;
                width: 90px;
                padding: 5px;
                outline: none;
            }
        }
    }
</style>