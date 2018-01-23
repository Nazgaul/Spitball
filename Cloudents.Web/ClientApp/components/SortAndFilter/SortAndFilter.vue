<template>
    <div class="sort-filter-wrap">
        <template v-if="sortOptions">
            <h3>Sort</h3>
            <div class="sort-switch">
                <template v-for="(o,index) in sortOptions">
                    <input type="radio" :id="`option${index}`" @click="updateSort(o.id)"
                           name="switch" :value="o.id" :checked="sortVal?sortVal===o.id:index===0">
                    <label :for="`option${index}`">{{o.name}}</label>
                </template>
            </div>
        </template>
        <div v-if="filterOptions">
            <h3>Filter</h3>
            <div class="filter-switch" v-if="filterOptions">
                <v-expansion-panel :value="true" expand>
                    <v-expansion-panel-content v-for="k in filterOptions" :key="k.modelId" :value="true">
                        <v-icon slot="actions" class="hidden-xs-only">sbf-chevron-down</v-icon>
                        <template slot="header">
                            <div class="icon-wrapper"><slot :name="`${k.modelId}TitlePrefix`"></slot></div>
                            <slot name="headerTitle" :title="k.title">
                                <div>{{k.title}}</div>
                            </slot>
                        </template>
                        <div class="sort-filter">
                            <div v-for="s in k.data" :key="(s.id?s.id:s)" class="filter">
                                <input type="checkbox" :id="(s.id?s.id:s)" :checked="filterVal.get(k.modelId)?filterVal.get(k.modelId).includes(s.id?s.id.toString():s.toString()):false"
                                       @change="filterCallback({id:k.modelId,val:(s.id?s.id.toString():s),type:$event})" />

                                <span class="checkmark"></span>
                                <label :title="s.name?s.name:s" :for="(s.id?s.id:s)">
                                    {{s.name?s.name:s | capitalize}}
                                </label>
                            </div>
                            <slot :name="`${k.modelId}EmptyState`" v-if="k.data&&k.data.length===0"></slot>
                            <slot :name="`${k.modelId}ExtraState`" v-else></slot>
                        </div>
                    </v-expansion-panel-content>
                </v-expansion-panel>
            </div>
        </div>
    </div>
</template>
<script>
    import {mapActions} from 'vuex'
    export default{
        props:{sortOptions:Array,sortVal:{},filterOptions:{},filterVal:{}},
        methods:{
            ...mapActions(['setFilteredCourses']),
            updateSort(val){
                this.$router.push({query:{...this.$route.query,sort:val}});
            },
            updateFilter({id,val,type}){
                let query=this.$route.query;
                let currentFilter=query[id];
                let model=Array.isArray(currentFilter)? currentFilter:[currentFilter];
                query[id]=[].concat([model,val]);
                if(!type.target.checked){
                    query[id]=query[id].filter(i=>i!==val);
                }
                if (val === 'inPerson' && type){query.sort="price"}
                if(id==='course'){this.setFilteredCourses(query.course)}
                this.$router.push({query});
            }
        }
    }
</script>
<style src="./SortAndFilter.less" lang="less"></style>


