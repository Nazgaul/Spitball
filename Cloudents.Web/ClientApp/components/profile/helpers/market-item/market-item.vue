<template>
    <v-flex xs12>

        <v-flex  v-for="(item, index) in items" 
            :key="`item${index}`"
            :style="{background: `url(${item.image})`}"
            class="market-item" :class="classItem" >
        
            <div class="header">
                 <div class="price">
                    <span>$39</span>
                </div>   

                <div class="votes">
                    <v-icon  class="upvotes">sbf-chevron-down</v-icon>
                    <span>3 Upvotes</span>
                </div>   
            </div>           
            
            <div class="item-info">
                <span>Sony Xperia</span>
            </div>    
            

        </v-flex>

    </v-flex> 
</template>

<style src="./market-item.less" lang="less"></style>
<script src="./market-item.js"></script>


