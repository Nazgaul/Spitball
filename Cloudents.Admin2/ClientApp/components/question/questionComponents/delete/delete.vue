<template>
        <form id="question-delete-form" @submit.prevent="deleteQ">
            <p>
                <span>{{title}}</span>
                <textarea v-model="questionsToDelete" name="model" id="delete_model" cols="33" rows="5" placeholder="question id's seperated with ';'" autofocus></textarea>
                <input type="submit" value="Submit"> 
            </p>
            
        </form>    
</template>

<script lang="ts" src='./delete.ts'></script>

<style lang="scss" scoped>
    form{
        width: 250px;
        margin: 0 auto;
        span{
            font-weight: bold;
            color: #5d5d5d;
        }
        p{
            display: flex;
            flex-flow: column;
            justify-content: center;
            text-align: center;
        }
        textarea{
            outline: none;
            border: none;
            background-color: #b6b6b6;
            color: #5d5d5d;
            font-weight: bold;
        }
        input{
            width:70px;
            justify-content: center;
            margin: 5px auto;
            background-color: #b6b6b6;
            outline: none;
            border: none;
            cursor: pointer;
            color: #5d5d5d;
            font-weight: bold;
            &:hover{
                background-color: #acacac;
            }
            &:active{
                background-color: #928e8e;
            }
        }
    }

</style>
