<template>
    <div class="become-second-wrap">
        <v-layout row wrap>
            <v-flex xs12 class="text-xs-center mb-4 mt-2">
                <span class="sharing-text">Sharing Your information and background will get more response from students</span>
            </v-flex>
            <v-flex xs12 :class="{'mt-3' : $vuetify.breakpoint.xsOnly}">
                <v-textarea
                        rows="2"
                        outline
                        v-model="about"
                        name="input-about"
                        placeholder="Type a Key sentence about yourself"
                        :label="'Opening sentence'"
                ></v-textarea>
            </v-flex>
            <v-flex xsw12>
                <v-textarea
                        rows="2"
                        outline
                        v-model="bio"
                        name="input-bio"
                        placeholder="Type a short paragraph about your background and who you are"
                        :label="'Bio'"
                ></v-textarea>
            </v-flex>
        </v-layout>
        <v-layout align-end justify-end class="mt-5 px-1">
            <v-btn
                   color="#4452FC"
                   class="white-text"
                   round
                   :disabled="btnDisabled"
                   @click="submitData()">
                <span>Done</span>
            </v-btn>
        </v-layout>
    </div>
</template>

<script>
    import {mapGetters, mapActions} from 'vuex';
    export default {
        name: "secondStep",
        data() {
            return {
                about: '',
                bio: ''
            };
        },
        computed: {
            ...mapGetters(['becomeTutorData']),
           btnDisabled(){
                return !this.about || !this.bio
           }
        },
        methods: {
            ...mapActions(['updateTutorInfo']),
            submitData() {
                let bioAbout = { bio: this.bio, about: this.about};
                let data = {...this.becomeTutorData, ...bioAbout };
                this.updateTutorInfo(data);
                this.$root.$emit('becomeTutorStep', 3);
            }
        },
    };
</script>

<style lang="less">
    @import '../../../styles/mixin.less';

    .become-second-wrap {

        .sharing-text {
            font-size: 14px;
            color: @textColor;
        }
        .v-text-field--outline > .v-input__control > .v-input__slot {
            border: solid 1px rgba(0, 0, 0, 0.19);
            &:hover {
                border: 1px solid rgba(0, 0, 0, 0.19) !important;
            }
        }
    }
</style>