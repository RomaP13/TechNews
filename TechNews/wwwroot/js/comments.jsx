class Comment extends React.Component {
    render() {
        const avatarLetter = this.props.author ? this.props.author.charAt(0).toUpperCase() : '?';
        return (
            <div className="d-flex mb-4">
                <div className="flex-shrink-0">
                    <div className="rounded-circle bg-dark text-white d-flex justify-content-center align-items-center" 
                         style={{width: '45px', height: '45px', fontSize: '20px', fontWeight: 'bold'}}>
                        {avatarLetter}
                    </div>
                </div>
                <div className="flex-grow-1 ms-3">
                    <div className="card border-0 shadow-sm">
                        <div className="card-body p-3">
                            <h6 className="card-title mb-1 fw-bold text-dark">
                                {this.props.author} 
                                <small className="text-muted fw-normal ms-2" style={{fontSize: '0.8rem'}}>{this.props.date}</small>
                            </h6>
                            <p className="card-text mt-2">{this.props.children}</p>
                        </div>
                    </div>
                </div>
            </div>
        );
    }
}

class CommentForm extends React.Component {
    constructor(props) {
        super(props);
        this.state = { text: '' };
        this.handleTextChange = this.handleTextChange.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
    }
    handleTextChange(e) {
        this.setState({ text: e.target.value });
    }
    handleSubmit(e) {
        e.preventDefault();
        var text = this.state.text.trim();
        if (!text) return;
        this.props.onCommentSubmit({ content: text });
        this.setState({ text: '' });
    }
    render() {
        // Якщо не авторизований - показуємо прохання увійти
        if (!window.isAuthenticated) {
            return (
                <div className="alert alert-warning shadow-sm border-0" role="alert">
                    <h5 className="alert-heading">Бажаєте обговорити новину?</h5>
                    <p className="mb-0">
                        Будь ласка, <a href="/Identity/Account/Login" className="alert-link">увійдіть</a> або <a href="/Identity/Account/Register" className="alert-link">зареєструйтесь</a>, щоб залишити коментар.
                    </p>
                </div>
            );
        }

        return (
            <form className="mb-5 p-4 bg-light rounded shadow-sm border" onSubmit={this.handleSubmit}>
                <h5 className="mb-3">Новий коментар від <b>{window.currentUser}</b></h5>
                <div className="form-group mb-3">
                    <textarea 
                        className="form-control border-0 shadow-sm" 
                        placeholder="Напишіть свою думку тут..." 
                        value={this.state.text}
                        onChange={this.handleTextChange}
                        rows="3"
                        style={{resize: 'none'}}
                    />
                </div>
                <div className="d-flex justify-content-end">
                    <input type="submit" className="btn btn-primary px-4 rounded-pill" value="Опублікувати" />
                </div>
            </form>
        );
    }
}

class CommentsBox extends React.Component {
    constructor(props) {
        super(props);
        this.state = { data: [] };
        this.handleCommentSubmit = this.handleCommentSubmit.bind(this);
    }
    loadCommentsFromServer() {
        var xhr = new XMLHttpRequest();
        xhr.open('get', this.props.apiUrl + "/" + this.props.postId, true);
        xhr.onload = function () {
            if (xhr.status === 200) {
                var data = JSON.parse(xhr.responseText);
                this.setState({ data: data });
            }
        }.bind(this);
        xhr.send();
    }
    handleCommentSubmit(comment) {
        var data = new FormData();
        data.append('content', comment.content);
        data.append('postId', this.props.postId);

        var xhr = new XMLHttpRequest();
        xhr.open('post', this.props.apiUrl, true);
        xhr.onload = function () {
            if (xhr.status === 200) {
                this.loadCommentsFromServer();
            } else if (xhr.status === 401) {
                // Якщо сесія закінчилась
                window.location.href = "/Identity/Account/Login";
            }
        }.bind(this);
        xhr.send(data);
    }
    componentDidMount() {
        this.loadCommentsFromServer();
        // Автооновлення коментарів кожні 5 сек
        this.interval = window.setInterval(() => this.loadCommentsFromServer(), 5000);
    }
    componentWillUnmount() {
        window.clearInterval(this.interval);
    }
    render() {
        return (
            <div className="comments-box">
                <h3 className="mb-4 pb-2 border-bottom">Обговорення <span className="badge bg-secondary rounded-pill fs-6 align-middle ms-2">{this.state.data.length}</span></h3>
                <CommentForm onCommentSubmit={this.handleCommentSubmit} />
                <CommentList data={this.state.data} />
            </div>
        );
    }
}

class CommentList extends React.Component {
    render() {
        if (!this.props.data || this.props.data.length === 0) {
            return <p className="text-muted text-center py-4">Поки немає коментарів. Станьте першим!</p>;
        }
        var commentNodes = this.props.data.map(function (comment) {
            return (
                <Comment key={comment.id} author={comment.authorEmail} date={comment.date}>
                    {comment.content}
                </Comment>
            );
        });
        return <div className="comment-list">{commentNodes}</div>;
    }
}

ReactDOM.render(
    <CommentsBox apiUrl="/api/CommentsApi" postId={window.postId} />,
    document.getElementById('react-comments')
);
