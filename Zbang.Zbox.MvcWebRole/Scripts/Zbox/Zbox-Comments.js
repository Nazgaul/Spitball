/// <reference path="/Scripts/jquery-1.6.2.js" />
Zbox.Comments = {
    commentItem: '',
    replyItem: '',
    commentReplyContainer: '',
    RenderComment: function (comment, addToContainerCallback, generateCommentElementIdFunction) {
        comment.CommentText = comment.CommentText.replace(/\n/g, '<br/>');
        if (this.commentItem == '') {
            var commenthtml = $('#commentTemplate').clone();

            //            var boxItemDetail = $('#BoxItemDetailTemplate').clone().html();
            //            $(commenthtml).find('.LocationBoxItemDetailTemplate').replaceWith(boxItemDetail);


            var actionDelete = $(commenthtml).find('#LocationdeleteCommentTemplate');
            if (comment.IsUserDeleteAllowed) {
                var commentActionDelete = $('#deleteCommentTemplate').clone().html();
                $(actionDelete).replaceWith(commentActionDelete);
            }
            else {
                $(actionDelete).remove();
            }
            this.commentItem = commenthtml;
            $('#commentTemplate').remove();
        }

        if (comment.ParentId == null) {
            addToContainerCallback(Zbox.changeTemplateText(this.commentItem.html(), comment));
        }
        else {
            if (this.replyItem == '') {
                this.replyItem = this.commentItem.clone();
                this.replyItem.find('div.comment-content-footer').remove();
                this.replyItem.find('div.replies').remove();
            }
            if (comment.ParentId == -1) {
                addToContainerCallback(Zbox.changeTemplateText(this.replyItem.html(), comment));
            }
            else {
                commentContainerIdentifier = 'comment' + comment.ParentId;
                //this.initColorBoxOnComment(commentContainerIdentifier);
                var repliesContainer = $('#' + commentContainerIdentifier).find('div.replies');
                repliesContainer.append(Zbox.changeTemplateText(this.replyItem.html(), comment));
            }
        }
     //   Zbox.UpdateScreenTime();

    }
}
Zbox.Comments.CommentTarget = function (title, url) {
    this.title = title;
    this.url = url;
};

Zbox.Comments.CommentTarget.Box = new Zbox.Comments.CommentTarget(
    'Post a box comment:',
    '/Comment/AddBoxComment'
);

Zbox.Comments.CommentTarget.Comment = new Zbox.Comments.CommentTarget(
    'Post a reply:',
    '/Comment/AddReply'
);

Zbox.Comments.PostComment = function (targetType, targetId, commentText, successCallback, errorCallback, boxId) {

    if (commentText == '') {
        errorCallback('please type a comment / reply');
        return;
    }
    var newCommentRequest = new ZboxAjaxRequest({
        url: targetType.url,
        data: { 'commentText': escape(commentText), 'targetId': targetId, 'boxId': boxId },
        success: function (comment) {
            successCallback(comment.comment, comment.boxId);
        },
        error: function (err) {
            errorCallback(err);
        }
    });
    newCommentRequest.Post();
};

Zbox.Comments.deleteComment = function (commentId, boxId) {
    var deleteCommentRequest = new ZboxAjaxRequest({
        url: '/Comment/DeleteComment',
        data: { commentId: commentId, boxId: boxId },
        success: function () {
            $('#comment' + commentId).remove();
        },
        error: function (err) {
        }
    });

    deleteCommentRequest.Post();
}

Zbox.Comments.initColorBoxComments = function () {
    $('#box-comments').find('.comment-content').find('a:not(.reply)').live('click', function (e) {
        //$('#box-comments .comment-content a:not(.reply)').live('click', function (e) {
        var $divFiles = $('#divFiles');
        var href = $(this).attr('data-href');
        var elementInFile = $divFiles.find('img[data-itemid="' + href + '"]');
        var removedItemName = $(this).text();
        var boxName = $('#selectedBoxName').text();
        if (elementInFile.length == 0) {
            e.preventDefault();
            Zbox.ShowConfirmDialog({
                title: '<img class="float-left icon-popupAlert" src="/Content/Images/icon-popupAlert.png"><div class="txt-18 blue-3 normal float-left">Item removed</div><br class="clear" />',
                message: '<div class="txt-13 blue-5">&quot;' + removedItemName + '&quot; was removed from the ' + boxName + ' box<br /> and is no longer available.</div>'
            });

        }
        elementInFile.click();
        //        if (Zbox.BoxItem.iscolorbox(href)) {

        //            var elementInFile = $divFiles.find('a[href="' + href + '"][rel="colorboxCombine"]');
        //            if (elementInFile.length == 1) {
        //                e.preventDefault();
        //                elementInFile.click();
        //            }
        //        }
        return false;
    });
}


//Comments.ascx
$(document).ready(function () {

    $('#box-comments').click(function (e) {
        var $e = $(e.target);
        //Open bubble to reply
        if ($e.hasClass('reply')) {
            var self = $e;
            Zbox.BoxItemAction.ActionPermissionAllowed(1, function () {
                //if (Zbox.BoxItemAction.CheckForPermission(1)) {
                var p;
                var $CommentReplyBubble = $('#CommentReplyBubble');
                if ($CommentReplyBubble.is(':visible')) {
                    $CommentReplyBubble.slideUp(200, function () {
                        p = $(this).detach();
                        attachBubble(p, self);
                    });
                }
                else {
                    p = $CommentReplyBubble.detach();
                    attachBubble(p, self);
                }
            });
            //            else {
            //                Zbox.InsufficentPermission();
            //            };
        }
        //submit reply
        if ($e.hasClass('submitReply')) {
            var commentContainer = $e.closest('div.comment');
            var commentId = commentContainer.attr('data-CommentId');
            var commentText = $('textarea.reply-bubble-text', commentContainer);

            Zbox.Comments.PostComment(
            Zbox.Comments.CommentTarget.Comment,
            commentId,
            commentText.val(),
            function (comment) {
                $('#CommentReplyBubble').slideUp();
                //$('.bubble', "#box-comments").slideUp();
                commentText.val('');
                Zbox.Comments.RenderComment(comment, function (html) {
                    commentContainer.find('div.replies').first().prepend(html);
                });
                Zbox.UpdateScreenTime();
            },
             Zbox.BoxItemAction.ShowError,
             $('#selecteBoxId').val());
        }
        if ($e.hasClass('delete-comment')) {
            //e.preventDefault();
            var commentContainer = $e.closest('div.comment');
            var commentId = commentContainer.attr('data-CommentId');
            var boxid = $('#selecteBoxId').val();
            var BoxNameTitle = $('#selectedBoxName').text();
            Zbox.ShowConfirmDialog({
                title: '<img class="float-left icon-popupAlert" src="/Content/Images/icon-popupAlert.png" /><div class="style80 float-left">Delete Comment</div><br class="clear" />',
                message: '<div class="style81">You are about to delete this comment from the ' + BoxNameTitle + ' box. </div>' +
                                      '<div class="style81">Are you sure?</div>',
                ok: function () {
                    Zbox.Comments.deleteComment(commentId, boxid);
                    return true;

                }
            });
        }
    });



    function attachBubble(p, self) {
        p.insertAfter($(self).parents('.comment-content').next());
        //            $(this).closest('.replies').prepend(p);
        p.slideDown(200, function () {
            resizeContext(document);
        });
        p.find('textarea.reply-bubble-text').focus();
    }


    $('#CommentReplyBubble').find('a.close-top-bubble').click(function () {
        $('#CommentReplyBubble').slideUp(200);
    });

    $('div.comment').live('mouseenter', function (event) {
        $('.deleteListIcon:first', this).css('display', 'block');
    });
    $('div.comment').live('mouseleave', function (event) {
        $('.deleteListIcon:first', this).css('display', 'none');
    });


});

//used for unreg page. need to remove it.
function loadBoxConversations(boxId, obj) {
    $('#box-comments-errors').hide();
    var $boxComments = $('#box-comments');
    var getBoxCommentsRequest = new ZboxAjaxRequest({
        beforeSend: function () {
            $boxComments.empty();
            $boxComments.attr('data-IsLoading', true);
            $boxComments.text('[Loading box conversations...]');
            //            if (Zbox.BoxItemAction.FireAjax() && obj === undefined) {
            //                Zbox.BoxItemAction.AjaxPending.comment = true;
            //                //Zbox.BoxItemAction.AjaxPending.AjaxGet.push(getBoxCommentsRequest);
            //                return false;
            //            }
        },
        url: '/Comment/GetBoxComments?boxId=' + boxId,
        success: function (comments) {
            loadBoxConversationSuccess(comments);
        },
        error: function (error) {
            $('#box-comments-errors').text(error).fadeIn('fast').delay(30000).fadeOut('slow');
        }
    });

    getBoxCommentsRequest.Get();

    //
    // Simon - hack for redraw 20110821
    //
    resizeContext(document); //resizeContext(document);
}


function loadBoxConversationSuccess(comments) {    
    if (comments.boxId == Zbox.Box.boxes.GetCurrentBox().shortUid) {
        
        var $boxComments = $('#box-comments');
        //var $CommentReplyBubble = $('#CommentReplyBubble').detach();
        $boxComments.empty();

        // $boxComments.append(Zbox.Comments.commentReplyContainer);
        var s = '';
        var length = comments.comments.length;
        for (var i = 0; i < length; i++) {
            Zbox.Comments.RenderComment(comments.comments[i], function (html) {
                //s += html;
                $boxComments.append(html);
            });
        }
        //$('#box-comments').html(s);        
    }

    Zbox.UpdateScreenTime();
    Zbox.Comments.initColorBoxComments();
}





