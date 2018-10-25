<template>
        <div class="select-university-container set-school" :class="{'selected': (!!search && search.length > 10) || !!university}">
            <div class="title-container">
                Select School
                <a v-show="search.length > 10 || !!university" class="next-container" @click="nextStep()">Next</a>  
            </div>
            <div class="select-school-container">
                <v-combobox
                    v-model="university"
                    :items="universities"
                    label="Type your school name"
                    placeholder="Type your school name"
                    clearable
                    solo
                    :search-input.sync="search"
                    :append-icon="''"
                    :clear-icon="'sbf-close'"
                    @click:clear="clearData()"
                    autofocus
                    no-filter
                >
                <template slot="no-data">
                    <v-list-tile v-show="showBox">
                        <div class="subheading">Keep typing we are searching for you</div> 
                    </v-list-tile>
                    <v-list-tile>
                        <div class="subheading dark">Show all Schools</div>
                    </v-list-tile>
                </template>
                <template slot="selection" slot-scope="{ item, parent, selected }">
                   <span style="color: rgba(0, 0, 0, 0.54);">{{!!item.text ? item.text : item}}</span> 
                </template>
                <template slot="item" slot-scope="{ index, item, parent }">
                    <v-list-tile-content>
                       <span v-html="$options.filters.boldText(item.text, search)">{{ item.text }}</span> 
                    </v-list-tile-content>
                </template>
                </v-combobox>
            </div>

            <div class="skip-container" v-if="$vuetify.breakpoint.xsOnly">
                <a @click="skipUniSelect()">Skip for now</a> 
            </div>

        </div>
</template>

<script>
import { mapGetters, mapActions } from 'vuex';
export default {
    props:{
        fnMethods:{
            required:true,
            type:Object
        },
        enumSteps:{
            required:true,
            type:Object
        }
    },
    data(){
        return{
            universityModel: '',
            search: ''
        }
    },
    watch:{
        search(val){
            if(val.length > 3){
                let uniArr = [];
                uniArr.push({text:'the collage of new jersey'})
                uniArr.push({text:'new jersey city'})
                uniArr.push({text:'new mexico state uni'})
                uniArr.push({text:'new mexico state wrerg'})
                uniArr.push({text:'new mexico state ertryr'})
                uniArr.push({text:'new mexico state rtyery'})
                uniArr.push({text:'new erty state uni'})
                uniArr.push({text:'new mexico state rtyry'})
                uniArr.push({text:'new eryry state uni'})
                this.updateUniversities(uniArr);
            }else if(val.length === 0 ){
                this.updateUniversities([]);
            }
        }
    },
    filters:{
        boldText(value, search){
            //mark the text bold according to the search value
            if (!value) return '';
            return value.replace(search, '<b>' + search + '</b>')
        }
    },
    methods:{
        ...mapActions(["updateUniversities", "clearUniversityList", "updateSchoolName", "changeSelectUniState"]),
        ...mapGetters(["getUniversities", "getSchoolName"]),
        clearData(){
            this.search = null;
            this.universityModel = undefined;
            this.clearUniversityList();
        },
        skipUniSelect(){
            this.changeSelectUniState(false);
        },
        nextStep(){
            let schoolName = !!this.universityModel.text ? this.universityModel.text : !!this.universityModel ? this.universityModel : this.search;
            if(!schoolName){
                //if the user went 1 step back, we should take the school name that was already set from the store
                schoolName = this.getSchoolName();
            }
            this.updateSchoolName(schoolName);
            this.fnMethods.changeStep(this.enumSteps.set_class);
        }
        
    },
    computed:{
        showBox(){
            return this.search.length > 0;
        },
        universities(){
            return this.getUniversities();
        },
        university:{
            get: function(){
                let schoolNameFromStore = this.getSchoolName();
                return schoolNameFromStore || this.universityModel
            },
            set: function(newValue){
                this.universityModel = newValue
            }
        }
    }
}
</script>

<style lang="less" scoped>
    .subheading{
        font-size: 16px;
        font-weight: normal;
        font-style: normal;
        font-stretch: normal;
        line-height: normal;
        letter-spacing: normal;
        color: rgba(0, 0, 0, 0.38);
        &.dark{
            font-size:13px!important;
            color: rgba(0, 0, 0, 0.54);
        }
    }
</style>
