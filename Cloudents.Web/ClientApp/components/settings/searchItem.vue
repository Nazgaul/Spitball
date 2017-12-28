
<template>
    <page-layout v-if="currentType" :type="currentType" :title="title" :search="!currentAction" :titleImage="(currentType==='course'&&!currentAction)?getUniversityImage:''" :isLoading="isLoading" :items="items" :selectedCourse="selectedCourse" :closeFunction="$_closeButton">
            <button class="done-btn" slot="courseExtraClose" @click="$emit('input',false)">DONE</button>
        <template slot="closeAction">
            <i v-if="isFirst&&currentType==='course'" class="sbf icon sbf-arrow-button"></i>
            <i v-else class="sbf icon sbf-close"></i>
        </template>
        
        <template v-if="showCreateCourse" slot="courseExtraItem">
            <div class="add-course-form ma-2 py-3 px-3">
                <form @submit.prevent="$_submitAddCourse" ref="addForm">
                    <div class="form-title">Still don't see your class?</div>
                        <v-text-field light v-model="newCourseName" placeholder="Type it in here:"></v-text-field>
                    <div class="actions">
                        <v-btn class="save" :disabled="!newCourseName" @click="$_submitAddCourse">save</v-btn>
                        <v-btn class="clear" :disabled="!newCourseName" @click="$_clearAddCourse">clear</v-btn>
                    </div>
                </form>
            </div>
        </template>

        <v-text-field light solo slot="inputField" @input="(val)=>{val.length > 2 ? isLoading = true : isLoading = false}" v-model.lazy="val" class="search-b elevation-0" ref="searchText" autofocus :placeholder="currentItem.placeholder" prepend-icon="sbf-search"></v-text-field>
        <v-chip class="ma-2" slot="selectedItems" slot-scope="props" v-if="selectedCourse" label>
            <span class="name" :title="props.course.name">{{ props.course.name }}</span>
            <button class="close pa-2" @click="$_removeCourse(props.course.id)">
                <i class="sbf icon sbf-close"></i>
            </button>
        </v-chip>
            <v-flex class="result mx-2 mt-3" v-if="!currentAction" @click="$_clickItemCallback(keep)" slot-scope="props" slot="results">
            
            <component :is="'search-item-'+currentType" :item="props.item"></component>
        </v-flex>
        <component slot="actionContent" v-if="currentAction" :is="currentType+'-'+currentAction" @done="$_actionDone"></component>
    </page-layout>
</template>
<script src="./searchItem.js">
</script>
<style src="./searchItem.less" lang="less"></style>
