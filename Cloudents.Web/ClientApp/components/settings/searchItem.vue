
<template>
    <page-layout v-if="currentType" :type="currentType" :title="title" :search="!currentAction" :showAdd="showAdd" :titleImage="(currentType==='course'&&!currentAction)?getUniversityImage:''" :isLoading="isLoading" :items="items" :selectedCourse="selectedCourse" :closeFunction="$_closeButton">
            <button class="done-btn" slot="courseExtraClose" @click="$emit('input',false)">DONE</button>
        <template slot="closeAction">
            <i v-if="isFirst&&currentType==='course'" class="sbf icon sbf-chevron-down"></i>
            <i v-else class="sbf icon sbf-close"></i>
        </template>
        
        <template v-if="showCreateCourse" slot="courseExtraItem">
            <div class="add-course-form mt-3 py-3 px-3" v-if="!isMobile">
                <form @submit.prevent="$_submitAddCourse">
                    <div class="form-title">Still don't see your class?</div>
                        <v-text-field light v-model="newCourseName" placeholder="Type it in here:"></v-text-field>
                    <div class="actions">
                        <v-btn class="save" :disabled="!newCourseName" @click="$_submitAddCourse">save</v-btn>
                        <v-btn class="clear" :disabled="!newCourseName" @click="$_clearAddCourse">clear</v-btn>
                    </div>
                </form>
            </div>
            <div v-else-if="!showAdd" @click.prevent="showAdd=true" class="add-course-form mr-2 pl-3 pb-3">
                <v-text-field light v-model="newCourseName" placeholder="Type it in here:" class="input-group--focused" label="Still don't see your class?"></v-text-field>
        </div>
        </template>
        <template v-if="showCreateCourse&&isMobile" slot="courseMobileExtraItem">
            <div class="add-course-dialog pt-4 pb-2 my-2 px-3">
                <form @submit.prevent="$_submitAddCourse">
                    <div class="form-title">Type it in here:</div>
                    <v-text-field light hide-details v-model="newCourseName" autofocus></v-text-field>
                    <div class="actions text-xs-right">
                        <v-btn class="clear" flat @click="$_clearAddCourse">Cancel</v-btn>
                        <v-btn class="save" flat :disabled="!newCourseName" @click="$_submitAddCourse">Save</v-btn>
                    </div>
                </form>
            </div>
        </template>


        <v-text-field light solo slot="inputField" @input="(val)=>{val.length > 2 ? isLoading = true : this.isLoading = false}" v-model.lazy="val" class="search-b" ref="searchText" autofocus :placeholder="currentItem.placeholder" prepend-icon="sbf-search"></v-text-field>
        <template  slot="selectedItems" slot-scope="props" v-if="selectedCourse">
            <v-chip v-if="!isMobile" label class="ma-2">
                <span class="name" :title="props.course.name">{{ props.course.name }}</span>
                <button class="close pa-2" @click="$_removeCourse(props.course.id)">
                    <i class="sbf icon sbf-close"></i>
                </button>
            </v-chip>
            <template v-else-if="!val">
                <component :is="'search-item-'+currentType" :item="props.course" :isChecked="true"></component>
            </template>
        </template>
            <v-flex class="result" v-if="!currentAction" @click="$_clickItemCallback(keep)" slot-scope="props" slot="results">
            
            <component :is="'search-item-'+currentType" :item="props.item"></component>
        </v-flex>
        <component slot="actionContent" v-if="currentAction" :is="currentType+'-'+currentAction" @done="$_actionDone"></component>
        <template slot="universityEmptyState" v-if="noResults">
            <div class="uni-empty-state">
                <div>Can't find your school?</div>
                <div>click <a  onclick="Intercom('showNewMessage')">here</a></div>
            </div>
        </template>
        <template slot="courseEmptyState" v-if="!items.length && !myCourses.length && !showCreateCourse">
            <div class="course-empty-state">You have not selected any courses</div>
        </template>
    </page-layout>
</template>
<script src="./searchItem.js">
</script>
<style src="./searchItem.less" lang="less"></style>
