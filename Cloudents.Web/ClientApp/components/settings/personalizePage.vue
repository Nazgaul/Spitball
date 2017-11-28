<template functional>
    <div>
        <div class="d-header">
            <v-layout row>
                <v-flex>
                    <slot name="closeAction"></slot>
                </v-flex>
                <v-flex>
                    <h1 class="mt-3">Personalize Results</h1>
                    <h5 v-if="props.title" class="pt-3">{{props.title}}</h5>
                </v-flex>
            </v-layout>
            <slot name="search" v-if="props.search">
                <v-container fluid grid-list-l> <v-layout row justify-center>
                    <v-flex xs4><slot name="inputField"></slot></v-flex></v-layout>
                    <v-container fluid slot="selectedItems" class="pa-0 mb-3" v-if="props.selectedCourse">
                        <v-layout row justify-center> <template v-for="course in props.selectedCourse"
                        >
                            <slot name="selectedItems" :course="course"></slot>
                        </template></v-layout>
                    </v-container>
                </v-container>
            </slot>
        </div>
        <div class="loader" v-if="props.search&&props.isLoading">
            <v-progress-circular indeterminate v-bind:size="50" color="amber"></v-progress-circular>
        </div>
        <v-container fluid class="d-result" v-else-if="props.search&&!props.isLoading">
            <v-list v-if="props.items.length">
                <template v-for="(item, index) in props.items">
                    <v-layout row justify-center>
                    <v-flex xs5><slot name="results" :item="item">
                    </slot></v-flex>
                    </v-layout>
                    <v-divider v-if="index < props.items.length-1"></v-divider>
                </template>
            </v-list><div v-else>
            <div>No Results Found</div>
            <div v-html="props.emptyText"></div>
        </div>
        </v-container>
        <slot name="actionContent">
        </slot>
    </div>
</template>
